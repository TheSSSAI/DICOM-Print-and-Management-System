using DMPS.Shared.Core.Common;
using DMPS.Shared.Core.Domain.Primitives;

namespace DMPS.Shared.Core.Domain.Entities;

/// <summary>
/// Represents a key-value pair for a global application setting.
/// This entity corresponds to the 'SystemSetting' table in the database.
/// Used for settings like data retention policies, password policies, etc.
/// </summary>
public sealed class SystemSetting : Entity<string>
{
    /// <summary>
    /// Gets the value of the setting.
    /// </summary>
    public string SettingValue { get; private set; }

    /// <summary>
    /// Gets an optional description of what the setting controls.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the timestamp of the last update to the setting.
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Private constructor for ORM materialization.
    /// </summary>
    private SystemSetting() : base(string.Empty) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemSetting"/> class.
    /// </summary>
    /// <param name="key">The unique key for the setting, which is also the ID.</param>
    /// <param name="value">The value for the setting.</param>
    /// <param name="description">An optional description for the setting.</param>
    private SystemSetting(string key, string value, string? description) : base(key)
    {
        SettingValue = value;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new system setting.
    /// </summary>
    /// <param name="key">The unique key for the setting.</param>
    /// <param name="value">The value for the setting.</param>
    /// <param name="description">An optional description.</param>
    /// <returns>A new <see cref="SystemSetting"/> instance.</returns>
    public static SystemSetting Create(string key, string value, string? description = null)
    {
        Guard.Against.NullOrWhiteSpace(key, nameof(key));
        Guard.Against.NullOrWhiteSpace(value, nameof(value));

        return new SystemSetting(key, value, description);
    }

    /// <summary>
    /// Updates the value of the setting.
    /// </summary>
    /// <param name="newValue">The new value.</param>
    public void UpdateValue(string newValue)
    {
        Guard.Against.NullOrWhiteSpace(newValue, nameof(newValue));
        SettingValue = newValue;
        UpdatedAt = DateTime.UtcNow;
    }
}