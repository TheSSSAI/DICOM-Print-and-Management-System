using DMPS.Data.Access.Repositories;
using DMPS.Infrastructure.IO.Abstractions;
using DMPS.Shared.Core.Commands;
using DMPS.Shared.Core.Enums;
using DMPS.Shared.Core.Exceptions;
using DMPS.Shared.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DMPS.Service.Worker.Handlers
{
    /// <summary>
    /// Handles the business logic for processing a single print job message from the queue.
    /// Registered as a singleton, it uses IServiceScopeFactory to manage scoped dependency lifetimes.
    /// </summary>
    public sealed class PrintJobMessageHandler
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<PrintJobMessageHandler> _logger;

        public PrintJobMessageHandler(IServiceScopeFactory serviceScopeFactory, ILogger<PrintJobMessageHandler> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        /// <summary>
        /// Processes a message containing a print job request.
        /// </summary>
        /// <param name="messageBody">The raw message body as a byte array.</param>
        /// <param name="correlationId">A unique identifier for tracing the operation.</param>
        /// <exception cref="MessageHandlerException">Thrown when a non-transient error occurs, signaling that the message should be dead-lettered.</exception>
        public async Task HandleAsync(byte[] messageBody, Guid correlationId)
        {
            ProcessPrintJobCommand? command = null;
            Guid printJobId = Guid.Empty;

            try
            {
                command = JsonSerializer.Deserialize<ProcessPrintJobCommand>(messageBody);
                if (command is null)
                {
                    throw new MessageHandlerException("Cannot deserialize message body into ProcessPrintJobCommand.", null, correlationId);
                }

                printJobId = command.PrintJobId;
                _logger.LogInformation("Starting print job processing for JobId: {PrintJobId}. CorrelationId: {CorrelationId}", printJobId, correlationId);

                await using var scope = _serviceScopeFactory.CreateAsyncScope();
                var printJobRepository = scope.ServiceProvider.GetRequiredService<IPrintJobRepository>();
                var printSpooler = scope.ServiceProvider.GetRequiredService<IPrintSpooler>();
                var auditLogRepository = scope.ServiceProvider.GetRequiredService<IAuditLogRepository>();

                // Step 1: Update job status to 'Processing'
                await printJobRepository.UpdateStatusAsync(printJobId, PrintJobStatus.Processing, null);

                // Step 2: Retrieve the full print job details which may include file paths and other metadata
                var printJob = await printJobRepository.GetByIdAsync(printJobId);
                if (printJob is null)
                {
                    throw new InvalidOperationException($"Print job with ID {printJobId} not found in the database.");
                }

                // Step 3: Spool the print job to the target printer
                // The IPrintSpooler is responsible for fetching DICOM files and rendering them.
                await printSpooler.SpoolJobAsync(printJob);

                // Step 4: Update job status to 'Completed'
                await printJobRepository.UpdateStatusAsync(printJobId, PrintJobStatus.Completed, null);

                // Step 5: Log the successful print event to the audit trail
                await auditLogRepository.LogEventAsync(new AuditLog
                {
                    EventType = "PrintJobCompleted",
                    EntityName = "PrintJob",
                    EntityId = printJobId.ToString(),
                    UserId = printJob.SubmittedByUserId,
                    Details = new { PrinterName = printJob.PrinterName, JobId = printJobId },
                    CorrelationId = correlationId
                });

                _logger.LogInformation("Successfully processed print job for JobId: {PrintJobId}. CorrelationId: {CorrelationId}", printJobId, correlationId);
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON Deserialization failed for print job message. CorrelationId: {CorrelationId}", correlationId);
                throw new MessageHandlerException("Message body is not a valid JSON for ProcessPrintJobCommand.", jsonEx, correlationId);
            }
            catch (PrinterException printerEx)
            {
                _logger.LogError(printerEx, "A printer-related error occurred for JobId: {PrintJobId}. CorrelationId: {CorrelationId}", printJobId, correlationId);
                await UpdateFailedJobStatusAsync(printJobId, $"Printer error: {printerEx.Message}");
                throw new MessageHandlerException("A non-recoverable printer error occurred.", printerEx, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during print job processing for JobId: {PrintJobId}. CorrelationId: {CorrelationId}", printJobId, correlationId);
                await UpdateFailedJobStatusAsync(printJobId, $"An unexpected error occurred: {ex.Message}");
                throw new MessageHandlerException("An unexpected error occurred during print job processing.", ex, correlationId);
            }
        }
        
        private async Task UpdateFailedJobStatusAsync(Guid printJobId, string reason)
        {
            if (printJobId == Guid.Empty) return;

            try
            {
                await using var scope = _serviceScopeFactory.CreateAsyncScope();
                var printJobRepository = scope.ServiceProvider.GetRequiredService<IPrintJobRepository>();
                await printJobRepository.UpdateStatusAsync(printJobId, PrintJobStatus.Failed, reason);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CRITICAL: Failed to update status for failed print job {PrintJobId}.", printJobId);
            }
        }
    }
}