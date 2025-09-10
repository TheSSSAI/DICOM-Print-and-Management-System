using DMPS.Shared.Core.Domain.Entities;

namespace DMPS.Shared.Core.Application.Contracts.Repositories;

/// <summary>
/// Defines the contract for a repository that handles data access for <see cref="Patient"/> entities.
/// </summary>
public interface IPatientRepository : IGenericRepository<Patient>
{
    /// <summary>
    /// Asynchronously finds a patient by their DICOM Patient ID.
    /// </summary>
    /// <param name="dicomPatientId">The DICOM Patient ID to search for.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains the <see cref="Patient"/> if found; otherwise, null.
    /// </returns>
    Task<Patient?> FindByDicomPatientIdAsync(string dicomPatientId);
}