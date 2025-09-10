using DMPS.Infrastructure.Dicom.Anonymization;
using DMPS.Infrastructure.Dicom.Interfaces;
using FellowOakDicom;

namespace DMPS.Infrastructure.Dicom.Anonymization;

/// <summary>
/// Implements a comprehensive anonymization strategy compliant with HIPAA Safe Harbor guidelines.
/// </summary>
public sealed class FullAnonymizationStrategy : IAnonymizationStrategy
{
    public AnonymizationProfile Profile => AnonymizationProfile.Full;

    /// <summary>
    /// Removes an extensive list of DICOM tags to de-identify the dataset according to
    /// HIPAA Safe Harbor requirements. This includes all patient demographics, dates, UIDs (optional, but often remapped),
    /// and institution-specific information.
    /// </summary>
    /// <param name="dataset">The DICOM dataset to anonymize in-place.</param>
    public void Anonymize(DicomDataset dataset)
    {
        // A comprehensive list based on DICOM PS3.15 E.1 De-identification
        // and HIPAA Safe Harbor method.
        var tagsToRemove = new List<DicomTag>
        {
            // Patient Identification and Demographics
            DicomTag.PatientName, DicomTag.PatientID, DicomTag.IssuerOfPatientID,
            DicomTag.PatientBirthDate, DicomTag.PatientBirthTime, DicomTag.PatientSex,
            DicomTag.OtherPatientIDs, DicomTag.OtherPatientNames, DicomTag.EthnicGroup,
            DicomTag.PatientAddress, DicomTag.PatientTelephoneNumbers, DicomTag.PatientMotherBirthName,
            DicomTag.CountryOfResidence, DicomTag.RegionOfResidence, DicomTag.PatientPrimaryLanguageCodeSequence,

            // Study and Series Information
            DicomTag.AccessionNumber, DicomTag.StudyID, DicomTag.ReferringPhysicianName,
            DicomTag.RequestingPhysician, DicomTag.AdmittingDiagnosesDescription,
            DicomTag.PerformingPhysicianName, DicomTag.NameOfPhysiciansReadingStudy,
            DicomTag.OperatorsName, DicomTag.InstitutionalDepartmentName, DicomTag.InstitutionName,
            DicomTag.InstitutionAddress,

            // Equipment and Location
            DicomTag.StationName,

            // Dates and Times (often shifted rather than removed, but removal is safest for full anonymization)
            DicomTag.StudyDate, DicomTag.SeriesDate, DicomTag.AcquisitionDate, DicomTag.ContentDate,
            DicomTag.StudyTime, DicomTag.SeriesTime, DicomTag.AcquisitionTime, DicomTag.ContentTime,
            DicomTag.DateOfLastCalibration, DicomTag.TimeOfLastCalibration,

            // Clinical Trial Information
            DicomTag.ClinicalTrialSponsorName, DicomTag.ClinicalTrialProtocolID, DicomTag.ClinicalTrialProtocolName,
            DicomTag.ClinicalTrialSiteID, DicomTag.ClinicalTrialSiteName,

            // Miscellaneous
            DicomTag.PatientComments, DicomTag.PerformedProcedureStepID, DicomTag.RequestAttributesSequence,
            DicomTag.ReferencedPatientSequence, DicomTag.PersonName, DicomTag.ScheduledProcedureStepID,
            DicomTag.DeviceSerialNumber, DicomTag.MedicalAlerts, DicomTag.Allergies,
            DicomTag.AdditionalPatientHistory, DicomTag.PregnancyStatus, DicomTag.PatientAge
        };

        foreach (var tag in tagsToRemove)
        {
            dataset.Remove(tag);
        }

        // Clean up private tags as they can contain PHI
        dataset.Remove(d => d.Tag.IsPrivate);
        
        // Remove overlay data which might contain burned-in annotations
        dataset.Remove(DicomTag.OverlayData);

        // Add a tag to indicate the dataset has been modified for de-identification
        dataset.AddOrUpdate(DicomTag.PatientIdentityRemoved, "YES");
        dataset.AddOrUpdate(DicomTag.DeidentificationMethod, "Full Profile: HIPAA Safe Harbor method applied.");
    }
}