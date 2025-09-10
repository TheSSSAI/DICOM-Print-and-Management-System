using DMPS.Data.Access.Repositories;
using DMPS.Infrastructure.IO.Abstractions;
using DMPS.Shared.Core.Commands;
using DMPS.Shared.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DMPS.Service.Worker.Handlers
{
    /// <summary>
    /// Handles the business logic for processing a single DICOM storage message from the queue.
    /// This class is registered as a singleton and uses IServiceScopeFactory to manage the lifecycle
    /// of scoped dependencies like repositories.
    /// </summary>
    public sealed class DicomStoreMessageHandler
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<DicomStoreMessageHandler> _logger;

        public DicomStoreMessageHandler(IServiceScopeFactory serviceScopeFactory, ILogger<DicomStoreMessageHandler> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        /// <summary>
        /// Processes a message containing DICOM study information for persistence.
        /// This involves moving files to permanent storage and writing metadata to the database transactionally.
        /// </summary>
        /// <param name="messageBody">The raw message body as a byte array.</param>
        /// <param name="correlationId">A unique identifier for tracing the operation.</param>
        /// <exception cref="MessageHandlerException">Thrown when a non-transient error occurs, signaling that the message should be dead-lettered.</exception>
        public async Task HandleAsync(byte[] messageBody, Guid correlationId)
        {
            ProcessDicomStoreCommand? command = null;
            try
            {
                command = JsonSerializer.Deserialize<ProcessDicomStoreCommand>(messageBody);
                if (command is null)
                {
                    _logger.LogError("Failed to deserialize ProcessDicomStoreCommand. Message body is invalid. CorrelationId: {CorrelationId}", correlationId);
                    // This is a non-recoverable error for this message.
                    throw new MessageHandlerException("Cannot deserialize message body into ProcessDicomStoreCommand.", null, correlationId);
                }

                _logger.LogInformation("Starting DICOM store processing for StudyInstanceUID: {StudyInstanceUID}. CorrelationId: {CorrelationId}", command.Study.StudyInstanceUid, correlationId);

                await using var scope = _serviceScopeFactory.CreateAsyncScope();
                var studyRepository = scope.ServiceProvider.GetRequiredService<IStudyRepository>();
                var dicomFileStorage = scope.ServiceProvider.GetRequiredService<IDicomFileStorage>();

                // Step 1: Move files from temporary/staging location to permanent, structured storage.
                // This operation needs to be idempotent. The infrastructure service should handle this.
                var permanentFilePaths = await dicomFileStorage.MoveToPermanentLocationAsync(command.StagedFilePaths, command.Study);

                // Step 2: Update the command's entity models with the new permanent file paths.
                UpdateImagePaths(command.Study, permanentFilePaths);

                // Step 3: Persist the entire study aggregate (Patient, Study, Series, Images) in a single database transaction.
                // The repository is responsible for handling idempotency (e.g., INSERT ON CONFLICT).
                await studyRepository.AddStudyTransactionAsync(command.Study);

                _logger.LogInformation("Successfully processed and stored StudyInstanceUID: {StudyInstanceUID}. CorrelationId: {CorrelationId}", command.Study.StudyInstanceUid, correlationId);
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON Deserialization failed for DICOM store message. CorrelationId: {CorrelationId}", correlationId);
                throw new MessageHandlerException("Message body is not a valid JSON for ProcessDicomStoreCommand.", jsonEx, correlationId);
            }
            catch (DicomValidationException validationEx)
            {
                _logger.LogError(validationEx, "DICOM data validation failed for StudyInstanceUID: {StudyInstanceUID}. CorrelationId: {CorrelationId}", command?.Study.StudyInstanceUid, correlationId);
                throw new MessageHandlerException("DICOM data validation failed.", validationEx, correlationId);
            }
            catch (IOException ioEx)
            {
                 _logger.LogError(ioEx, "File system error during DICOM store processing for StudyInstanceUID: {StudyInstanceUID}. CorrelationId: {CorrelationId}", command?.Study.StudyInstanceUid, correlationId);
                 // This could be transient (e.g., network share unavailable) or permanent (disk full).
                 // The message queue's retry policy will handle transient cases.
                 throw new MessageHandlerException("File system error occurred while moving DICOM files.", ioEx, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during DICOM store processing for StudyInstanceUID: {StudyInstanceUID}. CorrelationId: {CorrelationId}", command?.Study.StudyInstanceUid, correlationId);
                throw new MessageHandlerException("An unexpected error occurred during DICOM store processing.", ex, correlationId);
            }
        }
        
        private void UpdateImagePaths(Shared.Core.Models.Study study, IReadOnlyDictionary<string, string> permanentFilePaths)
        {
            foreach (var series in study.Series)
            {
                foreach (var image in series.Images)
                {
                    if (permanentFilePaths.TryGetValue(image.SopInstanceUid, out var newPath))
                    {
                        image.FilePath = newPath;
                    }
                    else
                    {
                        // This indicates a critical inconsistency and should fail the entire operation.
                        _logger.LogError("Could not find a permanent path for SOPInstanceUID {SopInstanceUid} in Study {StudyInstanceUid}", image.SopInstanceUid, study.StudyInstanceUid);
                        throw new InvalidOperationException($"Mismatch between staged files and permanent paths for SOPInstanceUID: {image.SopInstanceUid}");
                    }
                }
            }
        }
    }
}