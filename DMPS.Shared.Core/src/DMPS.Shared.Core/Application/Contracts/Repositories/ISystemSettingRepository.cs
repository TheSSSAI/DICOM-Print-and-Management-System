using DMPS.Shared.Core.Domain.Entities;

namespace DMPS.Shared.Core.Application.Contracts.Repositories;

/// <summary>
/// Defines the contract for a repository that handles data access for <see cref="SystemSetting"/> entities.
/// </summary>
public interface ISystemSettingRepository : IGenericRepository<SystemSetting>
{
    /// <summary>
    /// Asynchronously retrieves a system setting by its unique key.
    /// </summary>
    /// <param name="key">The key of the setting to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <see cref="SystemSetting"/> if found; otherwise, null.
    /// </returns>
    Task<SystemSetting?> GetByKeyAsync(string key);

    /// <summary>
    /// Asynchronously retrieves all system settings.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a read-only dictionary of all system settings.
    /// </returns>
    Task<IReadOnlyDictionary<string, SystemSetting>> GetAllAsDictionaryAsync();
}