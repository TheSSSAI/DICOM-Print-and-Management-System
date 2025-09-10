using DMPS.Infrastructure.Dicom.Anonymization;
using DMPS.Infrastructure.Dicom.Interfaces;
using FellowOakDicom;

namespace DMPS.Infrastructure.Dicom.Anonymization;

/// <summary>
/// Implements a basic anonymization strategy by removing common patient-identifying tags.
/// </summary>
public sealed class BasicAnonymizationStrategy : IAnonymizationStrategy
{
    public AnonymizationProfile Profile => AnonymizationProfile.Basic;

    /// <summary>
    /// Removes a predefined set of basic patient-identifying DICOM tags from the dataset.
    /// This includes Patient Name, ID, Birth Date, Sex, and Address.
    /// </summary>
    /// <param name="dataset">The DICOM dataset to anonymize in-place.</param>
    public void Anonymize(DicomDataset dataset)
    {
        // Patient Module Tags
        dataset.Remove(DicomTag.PatientName);
        dataset.Remove(DicomTag.PatientID);
        dataset.Remove(DicomTag.PatientBirthDate);
        dataset.Remove(DicomTag.PatientSex);
        dataset.Remove(DicomTag.PatientAge);
        dataset.Remove(DicomTag.PatientAddress);
        dataset.Remove(DicomTag.PatientTelephoneNumbers);
        dataset.Remove(DicomTag.OtherPatientIDs);
        dataset.Remove(DicomTag.OtherPatientNames);
        dataset.Remove(DicomTag.EthnicGroup);
        dataset.Remove(DicomTag.PatientComments);
        
        // General Study Module Tags
        dataset.Remove(DicomTag.AccessionNumber);
        dataset.Remove(DicomTag.ReferringPhysicianName);
        dataset.Remove(DicomTag.StudyID);
        dataset.Remove(DicomTag.RequestingPhysician);
        dataset.Remove(DicomTag.AdmittingDiagnosesDescription);
        
        // General Series Module
        dataset.Remove(DicomTag.PerformingPhysicianName);
        dataset.Remove(DicomTag.OperatorsName);
        
        // SOP Common Module
        dataset.Remove(DicomTag.InstitutionName);
        dataset.Remove(DicomTag.InstitutionAddress);
        
        // Add a tag to indicate the dataset has been modified for de-identification
        dataset.AddOrUpdate(DicomTag.PatientIdentityRemoved, "YES");
        dataset.AddOrUpdate(DicomTag.DeidentificationMethod, "Basic Profile: Patient demographics removed.");
    }
}