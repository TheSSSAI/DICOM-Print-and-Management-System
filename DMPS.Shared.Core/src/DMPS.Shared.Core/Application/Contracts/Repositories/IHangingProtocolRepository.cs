using DMPS.Shared.Core.Domain.Entities;

namespace DMPS.Shared.Core.Application.Contracts.Repositories;

/// <summary>
/// Defines the contract for a repository that handles data operations for HangingProtocol entities.
/// </summary>
public interface IHangingProtocolRepository : IGenericRepository<HangingProtocol, Guid>
{
    /// <summary>
    /// Asynchronously retrieves all hanging protocols created by a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains a collection of user-specific hanging protocols.
    /// </returns>
    Task<IEnumerable<HangingProtocol>> GetByUserIdAsync(Guid userId);

    /// <summary>
    /// Asynchronously retrieves all system-wide hanging protocols (those not assigned to a specific user).
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains a collection of system-wide hanging protocols.
    /// </returns>
    Task<IEnumerable<HangingProtocol>> GetSystemProtocolsAsync();

    /// <summary>
    /// Asynchronously finds all hanging protocols (both system and user-specific) that match the given criteria for automatic application.
    /// </summary>
    /// <param name="modality">The DICOM Modality tag value of the study.</param>
    /// <param name="bodyPartExamined">The DICOM Body Part Examined tag value of the study.</param>
    /// <param name="userId">The ID of the current user, to include their personal matching protocols.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains a collection of matching hanging protocols, which should be evaluated for the best fit.
    /// </returns>
    Task<IEnumerable<HangingProtocol>> FindMatchingProtocolsAsync(string modality, string bodyPartExamined, Guid userId);
}