using DMPS.Shared.Core.Common;
using DMPS.Shared.Core.Domain.Primitives;

namespace DMPS.Shared.Core.Domain.Entities;

/// <summary>
/// Represents a user role within the system, defining a set of permissions.
/// This entity corresponds to the 'Role' table in the database.
/// Fulfills requirement REQ-1-014.
/// </summary>
public sealed class Role : Entity<Guid>
{
    /// <summary>
    /// Gets the name of the role.
    /// </summary>
    public string RoleName { get; private set; }

    /// <summary>
    /// Gets the description of the role.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Private constructor for ORM materialization.
    /// </summary>
    private Role() : base(Guid.NewGuid()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Role"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the role.</param>
    /// <param name="roleName">The name of the role.</param>
    /// <param name="description">An optional description of the role.</param>
    private Role(Guid id, string roleName, string? description) : base(id)
    {
        RoleName = roleName;
        Description = description;
    }

    /// <summary>
    /// Creates a new Role entity.
    /// </summary>
    /// <param name="roleName">The name for the new role.</param>
    /// <param name="description">An optional description for the new role.</param>
    /// <returns>A new <see cref="Role"/> instance.</returns>
    public static Role Create(string roleName, string? description = null)
    {
        Guard.Against.NullOrWhiteSpace(roleName, nameof(roleName));

        return new Role(Guid.NewGuid(), roleName, description);
    }

    /// <summary>
    /// Updates the role's description.
    /// </summary>
    /// <param name="newDescription">The new description for the role.</param>
    public void UpdateDescription(string? newDescription)
    {
        Description = newDescription;
    }
}