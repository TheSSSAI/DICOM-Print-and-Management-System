using DMPS.Shared.Core.Application.Contracts.DTOs;
using DMPS.Shared.Core.Domain.Entities;

namespace DMPS.Shared.Core.Application.Contracts.Repositories;

/// <summary>
/// Defines the contract for a repository handling data access for Study entities.
/// </summary>
public interface IStudyRepository : IGenericRepository<Study, Guid>
{
    /// <summary>
    /// Asynchronously retrieves a study by its unique DICOM Study Instance UID.
    /// </summary>
    /// <param name="studyInstanceUid">The Study Instance UID to search for.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <see cref="Study"/> entity if found; otherwise, null.
    /// </returns>
    Task<Study?> GetStudyByInstanceUidAsync(string studyInstanceUid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously checks if a study with the given Study Instance UID exists.
    /// </summary>
    /// <param name="studyInstanceUid">The Study Instance UID to check for.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result is true if the study exists; otherwise, false.
    /// </returns>
    Task<bool> StudyExistsAsync(string studyInstanceUid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously finds studies based on a set of criteria.
    /// </summary>
    /// <param name="criteria">The criteria object used for filtering studies.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a collection of <see cref="Study"/> entities that match the criteria.
    /// </returns>
    Task<IEnumerable<Study>> FindStudiesAsync(DicomQueryCriteria criteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves studies older than a specified date for data retention policy enforcement.
    /// </summary>
    /// <param name="retentionDate">The cutoff date. Studies older than this date will be returned.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a collection of <see cref="Study"/> entities eligible for purging.
    /// </returns>
    Task<IEnumerable<Study>> GetStudiesForPurgingAsync(DateTime retentionDate, CancellationToken cancellationToken = default);
}