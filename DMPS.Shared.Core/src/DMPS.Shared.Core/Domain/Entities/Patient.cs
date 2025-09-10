using DMPS.Shared.Core.Domain.Primitives;

namespace DMPS.Shared.Core.Domain.Entities;

/// <summary>
/// Represents a patient, storing demographic information extracted from DICOM metadata.
/// All Protected Health Information (PHI) fields are intended to be encrypted at rest.
/// This entity corresponds to the 'Patient' table in the database.
/// Fulfills requirements REQ-1-010 and REQ-1-083.
/// </summary>
public sealed class Patient : Entity<Guid>, IAggregateRoot
{
    /// <summary>
    /// Gets the DICOM Patient ID (0010,0020).
    /// </summary>
    public string DicomPatientId { get; private set; }

    /// <summary>
    /// Gets the patient's full name (0010,0010).
    /// </summary>
    public string? PatientName { get; private set; }

    /// <summary>
    /// Gets the patient's date of birth (0010,0030).
    /// </summary>
    public string? PatientBirthDate { get; private set; }

    /// <summary>
    /// Gets the patient's sex (0010,0040).
    /// </summary>
    public string? PatientSex { get; private set; }
    
    /// <summary>
    /// Gets the timestamp of when the patient record was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the timestamp of the last update to the patient record.
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Private constructor for ORM materialization.
    /// </summary>
    private Patient() : base(Guid.NewGuid()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Patient"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the patient.</param>
    /// <param name="dicomPatientId">The DICOM Patient ID.</param>
    /// <param name="patientName">The patient's name.</param>
    /// <param name="patientBirthDate">The patient's birth date.</param>
    /// <param name="patientSex">The patient's sex.</param>
    private Patient(Guid id, string dicomPatientId, string? patientName, string? patientBirthDate, string? patientSex) 
        : base(id)
    {
        DicomPatientId = dicomPatientId;
        PatientName = patientName;
        PatientBirthDate = patientBirthDate;
        PatientSex = patientSex;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }
    
    /// <summary>
    /// Creates a new Patient entity.
    /// </summary>
    /// <param name="dicomPatientId">The DICOM Patient ID.</param>
    /// <param name="patientName">The patient's name.</param>
    /// <param name="patientBirthDate">The patient's birth date.</param>
    /// <param name="patientSex">The patient's sex.</param>
    /// <returns>A new instance of <see cref="Patient"/>.</returns>
    public static Patient Create(string dicomPatientId, string? patientName, string? patientBirthDate, string? patientSex)
    {
        Common.Guard.Against.NullOrWhiteSpace(dicomPatientId, nameof(dicomPatientId));
        return new Patient(Guid.NewGuid(), dicomPatientId, patientName, patientBirthDate, patientSex);
    }
    
    /// <summary>
    /// Updates the patient's demographic information.
    /// </summary>
    /// <param name="patientName">The updated patient name.</param>
    /// <param name="patientBirthDate">The updated patient birth date.</param>
    /// <param name="patientSex">The updated patient sex.</param>
    public void UpdateDemographics(string? patientName, string? patientBirthDate, string? patientSex)
    {
        PatientName = patientName;
        PatientBirthDate = patientBirthDate;
        PatientSex = patientSex;
        UpdatedAt = DateTime.UtcNow;
    }
}