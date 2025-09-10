using DMPS.Shared.Core.Entities;
using DMPS.Shared.Core.Repositories;
using DMPS.Data.Access.Contexts;
using Microsoft.EntityFrameworkCore;
using DMPS.Shared.Core.Exceptions;
using Npgsql;

namespace DMPS.Data.Access.Repositories;

public sealed class SystemSettingRepository : GenericRepository<SystemSetting>, ISystemSettingRepository
{
    public SystemSettingRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<SystemSetting?> GetByKeyAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        try
        {
            // Settings are often read, so AsNoTracking is appropriate.
            // FindAsync is efficient for primary key lookups.
            return await _dbSet.FindAsync(key);
        }
        catch (NpgsqlException ex)
        {
            throw new DataAccessException($"An error occurred while retrieving system setting with key '{key}'.", ex);
        }
    }
    
    /// <inheritdoc />
    public async Task<string?> GetValueByKeyAsync(string key, string? defaultValue = null)
    {
        var setting = await GetByKeyAsync(key);
        return setting?.SettingValue ?? defaultValue;
    }

    /// <inheritdoc />
    public async Task SetValueByKeyAsync(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Setting key cannot be null or whitespace.", nameof(key));
        if (value == null) throw new ArgumentNullException(nameof(value));

        try
        {
            var existingSetting = await _dbSet.FindAsync(key);

            if (existingSetting != null)
            {
                existingSetting.SettingValue = value;
                Update(existingSetting);
            }
            else
            {
                var newSetting = new SystemSetting
                {
                    SettingKey = key,
                    SettingValue = value,
                    Description = "Dynamically created setting." // Or manage descriptions elsewhere
                };
                await AddAsync(newSetting);
            }
        }
        catch (DbUpdateException ex)
        {
            throw new DataAccessException($"An error occurred while setting the value for system setting with key '{key}'.", ex);
        }
    }
}