using DMPS.Infrastructure.Communication.Abstractions;
using DMPS.Infrastructure.Dicom.Abstractions;
using DMPS.Service.Worker.Configuration;
using DMPS.Shared.Core.Commands;
using FellowOakDicom;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;

namespace DMPS.Service.Worker.Workers
{
    /// <summary>
    /// A background worker responsible for hosting the DICOM C-STORE SCP listener.
    /// It receives DICOM files from external modalities and publishes a command to a message queue for decoupled processing.
    /// </summary>
    public sealed class DicomScpListenerWorker : BackgroundService
    {
        private readonly ILogger<DicomScpListenerWorker> _logger;
        private readonly IDicomScpService _dicomScpService;
        private readonly IMessageProducer _messageProducer;
        private readonly DicomScpSettings _settings;

        public DicomScpListenerWorker(
            ILogger<DicomScpListenerWorker> logger,
            IOptions<DicomScpSettings> settings,
            IDicomScpService dicomScpService,
            IMessageProducer messageProducer)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(settings);
            ArgumentNullException.ThrowIfNull(dicomScpService);
            ArgumentNullException.ThrowIfNull(messageProducer);

            _logger = logger;
            _settings = settings.Value;
            _dicomScpService = dicomScpService;
            _messageProducer = messageProducer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DICOM SCP Listener Worker is starting.");

            stoppingToken.Register(() =>
                _logger.LogInformation("DICOM SCP Listener Worker is stopping."));

            try
            {
                await _dicomScpService.StartListeningAsync(
                    _settings.Port,
                    _settings.AETitle,
                    HandleDicomFileReceivedAsync,
                    stoppingToken);

                _logger.LogInformation("DICOM SCP Listener has started on port {Port} with AE Title {AETitle}",
                    _settings.Port, _settings.AETitle);

                // Keep the worker alive while the listener is running in the background.
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // This is expected when the service is stopping, so we don't log it as an error.
                _logger.LogInformation("DICOM SCP Listener Worker was cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "A fatal error occurred in the DICOM SCP Listener Worker, which is now stopping.");
            }
            finally
            {
                _logger.LogInformation("DICOM SCP Listener Worker has gracefully shut down.");
            }
        }

        /// <summary>
        /// Handles the event when a new DICOM file is received and stored temporarily by the SCP service.
        /// This method creates a command and publishes it to the message queue for asynchronous processing.
        /// </summary>
        /// <param name="dicomFile">The DICOM file object received.</param>
        /// <param name="tempFilePath">The temporary path where the file is stored.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task HandleDicomFileReceivedAsync(DicomFile dicomFile, string tempFilePath)
        {
            var correlationId = Guid.NewGuid();
            using var logScope = _logger.BeginScope("CorrelationId: {CorrelationId}", correlationId);

            try
            {
                _logger.LogInformation("Received DICOM file. Temp path: {TempPath}", tempFilePath);

                // Extract essential metadata for the command
                var studyInstanceUid = dicomFile.Dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty);
                var seriesInstanceUid = dicomFile.Dataset.GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, string.Empty);
                var sopInstanceUid = dicomFile.Dataset.GetSingleValueOrDefault(DicomTag.SOPInstanceUID, string.Empty);

                if (string.IsNullOrEmpty(studyInstanceUid) || string.IsNullOrEmpty(seriesInstanceUid) || string.IsNullOrEmpty(sopInstanceUid))
                {
                    _logger.LogWarning("Received DICOM file is missing essential UIDs. Discarding file: {TempPath}", tempFilePath);
                    // Optionally, delete the temp file if it's not managed by the SCP service
                    if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
                    return;
                }

                var command = new ProcessDicomStoreCommand
                {
                    TempFilePath = tempFilePath,
                    DicomMetadata = dicomFile.Dataset.ToDicomJson()
                };

                _logger.LogInformation("Publishing ProcessDicomStoreCommand for SOP Instance UID: {SopInstanceUid}", sopInstanceUid);

                await _messageProducer.PublishAsync(command, correlationId);

                _logger.LogDebug("Successfully published ProcessDicomStoreCommand for SOP Instance UID: {SopInstanceUid}", sopInstanceUid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle and publish received DICOM file from path {TempPath}", tempFilePath);
                // Depending on the desired reliability, you might implement a local retry or a fallback mechanism here.
                // For now, we log the error and move on, relying on the source modality to resend if necessary.
            }
        }
    }
}