namespace DMPS.Infrastructure.Dicom.Anonymization;

/// <summary>
/// Specifies the different levels of de-identification that can be applied to a DICOM file.
/// </summary>
public enum AnonymizationProfile
{
    /// <summary>
    /// Removes primary patient identifying information such as Name, ID, and Birth Date.
    /// </summary>
    Basic,

    /// <summary>
    /// Removes all patient, physician, and institution related information, as well as private tags,
    /// in compliance with HIPAA Safe Harbor guidelines.
    /// </summary>
    Full
}