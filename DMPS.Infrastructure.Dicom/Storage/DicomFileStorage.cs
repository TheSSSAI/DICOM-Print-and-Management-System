using DMPS.Infrastructure.Dicom.Exceptions;
using DMPS.Infrastructure.Dicom.Interfaces;
using FellowOakDicom;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace DMPS.Infrastructure.Dicom.Storage;

/// <summary>
/// Implements DICOM file storage logic, managing persistence to the file system
/// in a structured, hierarchical format.
/// </summary>
public sealed class DicomFileStorage : IDicomFileStorage
{
    private readonly ILogger<DicomFileStorage> _logger;

    public DicomFileStorage(ILogger<DicomFileStorage> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Stores a DICOM file in a hierarchical path based on its metadata.
    /// The path structure is: [storageRoot]\[PatientID]\[StudyInstanceUID]\[SeriesInstanceUID]\[SOPInstanceUID].dcm
    /// </summary>
    /// <param name="file">The DICOM file to store.</param>
    /// <param name="storageRoot">The root directory for DICOM storage.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The full path to the stored file.</returns>
    public async Task<string> StoreFileAsync(DicomFile file, string storageRoot, CancellationToken cancellationToken = default)
    {
        if (file is null) throw new ArgumentNullException(nameof(file));
        if (string.IsNullOrWhiteSpace(storageRoot)) throw new ArgumentException("Storage root path cannot be null or whitespace.", nameof(storageRoot));

        try
        {
            var dataset = file.Dataset;
            var patientId = GetRequiredTagValue(dataset, DicomTag.PatientID);
            var studyUid = GetRequiredTagValue(dataset, DicomTag.StudyInstanceUID);
            var seriesUid = GetRequiredTagValue(dataset, DicomTag.SeriesInstanceUID);
            var sopInstanceUid = file.FileMetaInfo.MediaStorageSOPInstanceUID.UID;

            if (string.IsNullOrWhiteSpace(sopInstanceUid))
            {
                // Fallback to dataset tag if not in meta info
                sopInstanceUid = GetRequiredTagValue(dataset, DicomTag.SOPInstanceUID);
            }

            // Sanitize identifiers to be safe for directory names, although UIDs and IDs are typically safe.
            patientId = SanitizePathComponent(patientId);

            var seriesDirectory = Path.Combine(storageRoot, patientId, studyUid, seriesUid);
            Directory.CreateDirectory(seriesDirectory);

            var filePath = Path.Combine(seriesDirectory, $"{sopInstanceUid}.dcm");

            await file.SaveAsync(filePath);

            _logger.LogDebug("Successfully stored DICOM file at {FilePath}", filePath);

            return filePath;
        }
        catch (DicomStorageException)
        {
            // Re-throw exceptions related to missing tags
            throw;
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "An I/O error occurred while trying to store a DICOM file.");
            throw new DicomStorageException("Failed to store DICOM file due to an I/O error. Check disk space and permissions.", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "An access error occurred while trying to store a DICOM file.");
            throw new DicomStorageException($"Access denied while trying to store DICOM file in '{storageRoot}'. Check service account permissions.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during DICOM file storage.");
            throw new DicomStorageException("An unexpected error occurred while storing the DICOM file.", ex);
        }
    }

    /// <summary>
    /// Retrieves a DICOM file from the specified path.
    /// </summary>
    /// <param name="filePath">The full path to the DICOM file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The loaded DicomFile object.</returns>
    public async Task<DicomFile> GetFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("File path cannot be null or whitespace.", nameof(filePath));

        try
        {
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Attempted to access a DICOM file that does not exist at path: {FilePath}", filePath);
                throw new FileNotFoundException("The specified DICOM file was not found.", filePath);
            }
            
            return await DicomFile.OpenAsync(filePath);
        }
        catch (FileNotFoundException)
        {
            // Re-throw to be specific
            throw;
        }
        catch(DicomFileException ex)
        {
            _logger.LogError(ex, "Failed to open or parse DICOM file at {FilePath}", filePath);
            throw new DicomStorageException($"The file at '{filePath}' could not be opened as a valid DICOM file.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving DICOM file from {FilePath}", filePath);
            throw new DicomStorageException("An unexpected error occurred while retrieving the DICOM file.", ex);
        }
    }

    private string GetRequiredTagValue(DicomDataset dataset, DicomTag tag)
    {
        if (!dataset.TryGetSingleValue(tag, out string value) || string.IsNullOrWhiteSpace(value))
        {
            _logger.LogError("Required DICOM tag {Tag} ({TagName}) is missing or empty. Cannot store file.", tag.ToString(), tag.DictionaryEntry.Name);
            throw new DicomStorageException($"Required DICOM tag for storage path construction is missing: {tag.DictionaryEntry.Name} ({tag}).");
        }
        return value;
    }

    private string SanitizePathComponent(string component)
    {
        foreach (char invalidChar in Path.GetInvalidFileNameChars())
        {
            component = component.Replace(invalidChar, '_');
        }
        return component;
    }
}