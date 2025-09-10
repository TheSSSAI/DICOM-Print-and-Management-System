using DMPS.Shared.Core.Common;
using DMPS.Shared.Core.Domain.Primitives;

namespace DMPS.Shared.Core.Domain.Entities;

/// <summary>
/// Represents a key-value store for user-specific preferences, 
/// such as custom WW/WL presets or UI settings.
/// </summary>
public sealed class UserPreference : Entity<Guid>
{
    /// <summary>
    /// The ID of the user to whom this preference belongs.
    /// Forms part of a composite key with PreferenceKey in the database.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// The unique key for the preference (e.g., 'DefaultAnnotationColor').
    /// Forms part of a composite key with UserId in the database.
    /// </summary>
    public string PreferenceKey { get; private set; } = string.Empty;

    /// <summary>
    /// The value of the preference, stored as a string.
    /// </summary>
    public string PreferenceValue { get; private set; } = string.Empty;

    /// <summary>
    /// The timestamp of the last update to the preference.
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Navigation property to the user who owns this preference.
    /// </summary>
    public User? User { get; private set; }

    /// <summary>
    /// Private constructor for Entity Framework Core.
    /// </summary>
    private UserPreference() : base(Guid.NewGuid()) { }

    /// <summary>
    /// Creates a new instance of the <see cref="UserPreference"/> entity.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="key">The preference key.</param>
    /// <param name="value">The preference value.</param>
    /// <returns>A new <see cref="UserPreference"/> instance.</returns>
    public static UserPreference Create(Guid userId, string key, string value)
    {
        Guard.Against.Default(userId, nameof(userId));
        Guard.Against.NullOrWhiteSpace(key, nameof(key));
        Guard.Against.Null(value, nameof(value));

        return new UserPreference
        {
            UserId = userId,
            PreferenceKey = key,
            PreferenceValue = value,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Updates the value of the preference.
    /// </summary>
    /// <param name="newValue">The new value to set.</param>
    public void UpdateValue(string newValue)
    {
        Guard.Against.Null(newValue, nameof(newValue));
        PreferenceValue = newValue;
        UpdatedAt = DateTime.UtcNow;
    }
}