// DMPS.Shared.Core/Application/Contracts/DTOs/DicomQueryCriteria.cs
namespace DMPS.Shared.Core.Application.Contracts.DTOs
{
    /// <summary>
    /// A Data Transfer Object that encapsulates the criteria for a DICOM C-FIND query.
    /// </summary>
    /// <param name="PatientName">The patient's name to query. Supports wildcards (*, ?).</param>
    /// <param name="PatientId">The patient's ID to query.</param>
    /// <param name="StudyDateFrom">The start of the study date range for the query.</param>
    /// <param name="StudyDateTo">The end of the study date range for the query.</param>
    /// <param name="Modalities">A list of modalities to include in the query (e.g., "CT", "MR").</param>
    /// <param name="AccessionNumber">The accession number for the study.</param>
    public sealed record DicomQueryCriteria(
        string? PatientName,
        string? PatientId,
        DateOnly? StudyDateFrom,
        DateOnly? StudyDateTo,
        List<string>? Modalities,
        string? AccessionNumber);
}