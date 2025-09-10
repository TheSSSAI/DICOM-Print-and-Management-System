using DMPS.Shared.Core.Common;
using DMPS.Shared.Core.Domain.Primitives;

namespace DMPS.Shared.Core.Domain.Entities;

/// <summary>
/// Represents a historical record of a user's password hash.
/// This entity is part of the User aggregate and is used to enforce password reuse policies.
/// </summary>
public sealed class PasswordHistory : Entity<Guid>
{
    /// <summary>
    /// Foreign key referencing the user to whom this password history belongs.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// The salted hash of a previously used password.
    /// </summary>
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>
    /// The timestamp when this password was set and became historical.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Navigation property to the associated User.
    /// This is for navigational purposes in the domain model and may not always be loaded.
    /// </summary>
    public User? User { get; private set; }

    /// <summary>
    /// Private constructor for Entity Framework Core.
    /// </summary>
    private PasswordHistory() : base(Guid.NewGuid()) { }

    /// <summary>
    /// Creates a new instance of the <see cref="PasswordHistory"/> entity.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="passwordHash">The hashed password to store.</param>
    /// <returns>A new <see cref="PasswordHistory"/> instance.</returns>
    public static PasswordHistory Create(Guid userId, string passwordHash)
    {
        Guard.Against.Default(userId, nameof(userId));
        Guard.Against.NullOrWhiteSpace(passwordHash, nameof(passwordHash));

        return new PasswordHistory
        {
            UserId = userId,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };
    }
}