using DMPS.Infrastructure.Dicom.Anonymization;
using FellowOakDicom;

namespace DMPS.Infrastructure.Dicom.Interfaces;

/// <summary>
/// Defines the contract for a service that anonymizes DICOM files.
/// </summary>
public interface IDicomAnonymizer
{
    /// <summary>
    /// Anonymizes a DICOM file based on a specified anonymization profile.
    /// This operation is non-destructive; it returns a new, anonymized copy of the original file.
    /// </summary>
    /// <param name="original">The original DICOM file to anonymize.</param>
    /// <param name="profile">The anonymization profile to apply.</param>
    /// <returns>A new DicomFile instance containing the anonymized data.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the original file is null.</exception>
    /// <exception cref="NotSupportedException">Thrown if the specified anonymization profile is not supported.</exception>
    DicomFile Anonymize(DicomFile original, AnonymizationProfile profile);
}