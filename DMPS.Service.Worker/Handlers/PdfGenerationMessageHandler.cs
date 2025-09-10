using DMPS.Infrastructure.IO.Abstractions;
using DMPS.Shared.Core.Commands;
using DMPS.Shared.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DMPS.Service.Worker.Handlers
{
    /// <summary>
    /// Handles the business logic for processing a single PDF generation message from the queue.
    /// Registered as a singleton, it uses IServiceScopeFactory to manage scoped dependency lifetimes.
    /// </summary>
    public sealed class PdfGenerationMessageHandler
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<PdfGenerationMessageHandler> _logger;

        public PdfGenerationMessageHandler(IServiceScopeFactory serviceScopeFactory, ILogger<PdfGenerationMessageHandler> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        /// <summary>
        /// Processes a message containing a PDF generation request.
        /// </summary>
        /// <param name="messageBody">The raw message body as a byte array.</param>
        /// <param name="correlationId">A unique identifier for tracing the operation.</param>
        /// <exception cref="MessageHandlerException">Thrown when a non-transient error occurs, signaling that the message should be dead-lettered.</exception>
        public async Task HandleAsync(byte[] messageBody, Guid correlationId)
        {
            GeneratePdfCommand? command = null;
            try
            {
                command = JsonSerializer.Deserialize<GeneratePdfCommand>(messageBody);
                if (command is null)
                {
                    throw new MessageHandlerException("Cannot deserialize message body into GeneratePdfCommand.", null, correlationId);
                }

                _logger.LogInformation("Starting PDF generation for output path: {OutputPath}. CorrelationId: {CorrelationId}", command.OutputFilePath, correlationId);

                await using var scope = _serviceScopeFactory.CreateAsyncScope();
                var pdfGenerator = scope.ServiceProvider.GetRequiredService<IPdfGenerator>();
                var fileStorage = scope.ServiceProvider.GetRequiredService<IFileStorage>();
                var auditLogRepository = scope.ServiceProvider.GetRequiredService<IAuditLogRepository>();

                // Step 1: Generate the PDF document in memory using the provided layout and data
                var pdfBytes = await pdfGenerator.GeneratePdfAsync(command.LayoutDefinition);

                // Step 2: Save the generated PDF file to the user-specified location
                await fileStorage.SaveFileAsync(pdfBytes, command.OutputFilePath);

                // Step 3: Log the successful export event to the audit trail
                await auditLogRepository.LogEventAsync(new Shared.Core.Models.AuditLog
                {
                    EventType = "PdfExportCompleted",
                    EntityName = "Study", 
                    EntityId = command.LayoutDefinition.StudyInstanceUid,
                    UserId = command.RequestingUserId,
                    Details = new { OutputPath = command.OutputFilePath, FileSize = pdfBytes.Length },
                    CorrelationId = correlationId
                });

                _logger.LogInformation("Successfully generated and saved PDF to {OutputPath}. CorrelationId: {CorrelationId}", command.OutputFilePath, correlationId);
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON Deserialization failed for PDF generation message. CorrelationId: {CorrelationId}", correlationId);
                throw new MessageHandlerException("Message body is not a valid JSON for GeneratePdfCommand.", jsonEx, correlationId);
            }
            catch (UnauthorizedAccessException authEx)
            {
                _logger.LogError(authEx, "Permission denied while saving PDF to {OutputPath}. CorrelationId: {CorrelationId}", command?.OutputFilePath, correlationId);
                throw new MessageHandlerException($"Permission denied for path: {command?.OutputFilePath}", authEx, correlationId);
            }
            catch (IOException ioEx)
            {
                _logger.LogError(ioEx, "File system error while saving PDF to {OutputPath}. CorrelationId: {CorrelationId}", command?.OutputFilePath, correlationId);
                throw new MessageHandlerException("A file system error occurred while saving the PDF.", ioEx, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during PDF generation for {OutputPath}. CorrelationId: {CorrelationId}", command?.OutputFilePath, correlationId);
                throw new MessageHandlerException("An unexpected error occurred during PDF generation.", ex, correlationId);
            }
        }
    }
}