using FellowOakDicom;

namespace DMPS.Infrastructure.Dicom.Interfaces;

/// <summary>
/// Defines the contract for storing and retrieving DICOM files from a physical storage medium.
/// </summary>
public interface IDicomFileStorage
{
    /// <summary>
    /// Stores a DICOM file to the specified storage root using a hierarchical path structure.
    /// The path is constructed as: {storageRoot}/{PatientID}/{StudyInstanceUID}/{SeriesInstanceUID}/{SOPInstanceUID}.dcm
    /// </summary>
    /// <param name="file">The DICOM file to store.</param>
    /// <param name="storageRoot">The root directory for DICOM storage.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the full path to the stored file.</returns>
    /// <exception cref="DicomStorageException">Thrown if required DICOM tags for path construction are missing or if a file system error occurs.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the file or storageRoot is null.</exception>
    Task<string> StoreFileAsync(DicomFile file, string storageRoot, CancellationToken cancellationToken = default);
}