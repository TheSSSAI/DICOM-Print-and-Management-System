using DMPS.Client.Application.DTOs;
using DMPS.Client.Application.Interfaces;
using DMPS.Infrastructure.Communication.Abstractions;
using DMPS.Shared.Contracts.Commands;
using Microsoft.Extensions.Logging;

namespace DMPS.Client.Application.Services
{
    /// <summary>
    /// Implements the client-side logic for submitting asynchronous print and export jobs.
    /// This service acts as a facade, constructing command messages and publishing them
    /// to a message broker for backend processing.
    /// </summary>
    public sealed class PrintJobService : IPrintJobService
    {
        private readonly IMessageProducer _messageProducer;
        private readonly ILogger<PrintJobService> _logger;

        public PrintJobService(IMessageProducer messageProducer, ILogger<PrintJobService> logger)
        {
            _messageProducer = messageProducer ?? throw new ArgumentNullException(nameof(messageProducer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task SubmitPrintJobAsync(PrintJobData jobData)
        {
            ArgumentNullException.ThrowIfNull(jobData);

            var correlationId = Guid.NewGuid();
            _logger.LogInformation("Submitting new print job. Correlation ID: {CorrelationId}, Printer: {PrinterName}",
                correlationId, jobData.Destination);

            try
            {
                var command = new SubmitPrintJobCommand
                {
                    LayoutDefinition = jobData.LayoutDefinition,
                    ImageSopInstanceUids = jobData.ImageSopInstanceUids,
                    PrinterName = jobData.Destination,
                    SubmittedByUserId = jobData.SubmittedByUserId
                };

                var properties = new MessageProperties { CorrelationId = correlationId };

                await _messageProducer.PublishAsync(command, "print.job.request", properties);

                _logger.LogInformation("Successfully published print job command with Correlation ID: {CorrelationId}", correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish print job command. Correlation ID: {CorrelationId}", correlationId);
                // Re-throw a more specific, application-level exception if needed, or let a global handler manage it.
                // For now, we propagate the original exception to allow UI to handle infrastructure failures.
                throw;
            }
        }

        /// <inheritdoc />
        public async Task ExportToPdfAsync(PrintJobData jobData)
        {
            ArgumentNullException.ThrowIfNull(jobData);

            var correlationId = Guid.NewGuid();
            _logger.LogInformation("Submitting new PDF export job. Correlation ID: {CorrelationId}, Output Path: {OutputPath}",
                correlationId, jobData.Destination);

            try
            {
                var command = new GeneratePdfCommand
                {
                    LayoutDefinition = jobData.LayoutDefinition,
                    ImageSopInstanceUids = jobData.ImageSopInstanceUids,
                    OutputFilePath = jobData.Destination,
                    SubmittedByUserId = jobData.SubmittedByUserId,
                    EncryptionPassword = jobData.EncryptionPassword // Assuming this is part of PrintJobData
                };

                var properties = new MessageProperties { CorrelationId = correlationId };

                await _messageProducer.PublishAsync(command, "pdf.generation.request", properties);

                _logger.LogInformation("Successfully published PDF export command with Correlation ID: {CorrelationId}", correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish PDF export command. Correlation ID: {CorrelationId}", correlationId);
                throw;
            }
        }
    }
}