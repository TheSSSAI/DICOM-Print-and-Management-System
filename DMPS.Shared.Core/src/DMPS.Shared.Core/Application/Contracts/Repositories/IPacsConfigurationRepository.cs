using DMPS.Shared.Core.Domain.Entities;

namespace DMPS.Shared.Core.Application.Contracts.Repositories;

/// <summary>
/// Defines the contract for a repository that handles data access for <see cref="PacsConfiguration"/> entities.
/// </summary>
public interface IPacsConfigurationRepository : IGenericRepository<PacsConfiguration>
{
    // C-FIND, C-MOVE, and C-STORE requirements are complex and will likely be orchestrated
    // by an application service that uses the basic CRUD operations provided by IGenericRepository.
    // Specific query methods can be added here if complex search criteria for PACS configurations arise.
    // For example: Task<IEnumerable<PacsConfiguration>> GetConfigurationsByModalitySupportAsync(string modality);
}