using DMPS.Shared.Core.Domain.Primitives;
using System;
using DMPS.Shared.Core.Common;

namespace DMPS.Shared.Core.Domain.Entities
{
    /// <summary>
    /// Represents a system user. This is the aggregate root for user-related data.
    /// It encapsulates authentication, profile, and authorization information.
    /// This entity is central to fulfilling requirements like REQ-1-014 (RBAC) and REQ-1-111 (Forced Password Change).
    /// </summary>
    public class User : Entity<Guid>, IAggregateRoot
    {
        /// <summary>
        /// Gets the unique username for login.
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Gets the salted BCrypt hash of the user's password.
        /// </summary>
        public string PasswordHash { get; private set; }

        /// <summary>
        /// Gets the user's first name.
        /// </summary>
        public string FirstName { get; private set; }

        /// <summary>
        /// Gets the user's last name.
        /// </summary>
        public string LastName { get; private set; }

        /// <summary>
        /// Gets the foreign key to the associated Role entity.
        /// </summary>
        public Guid RoleId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the user account is active and can log in.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the user must change their password on the next login.
        /// Fulfills requirement REQ-1-111.
        /// </summary>
        public bool IsTemporaryPassword { get; private set; }

        /// <summary>
        /// Gets the timestamp of the last password change, used for expiration policy enforcement.
        /// Fulfills requirement REQ-1-110.
        /// </summary>
        public DateTime PasswordLastChangedAt { get; private set; }

        /// <summary>
        /// Gets the timestamp of when the user was created.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets the timestamp of the last update to the user record.
        /// </summary>
        public DateTime UpdatedAt { get; private set; }
        
        /// <summary>
        /// Private constructor for EF Core.
        /// </summary>
        private User() : base(Guid.NewGuid()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="id">The unique identifier for the user.</param>
        /// <param name="username">The unique username for login.</param>
        /// <param name="passwordHash">The salted BCrypt hash of the user's password.</param>
        /// <param name="firstName">The user's first name.</param>
        /// <param name="lastName">The user's last name.</param>
        /// <param name="roleId">The identifier of the user's assigned role.</param>
        public User(Guid id, string username, string passwordHash, string firstName, string lastName, Guid roleId) : base(id)
        {
            Username = Guard.Against.NullOrWhiteSpace(username, nameof(username));
            PasswordHash = Guard.Against.NullOrWhiteSpace(passwordHash, nameof(passwordHash));
            FirstName = Guard.Against.NullOrWhiteSpace(firstName, nameof(firstName));
            LastName = Guard.Against.NullOrWhiteSpace(lastName, nameof(lastName));
            RoleId = Guard.Against.Default(roleId, nameof(roleId));

            IsActive = true;
            IsTemporaryPassword = true; // New users must change their password
            var now = DateTime.UtcNow;
            PasswordLastChangedAt = now;
            CreatedAt = now;
            UpdatedAt = now;
        }

        /// <summary>
        /// Creates a new user with system-generated values.
        /// </summary>
        public static User Create(string username, string passwordHash, string firstName, string lastName, Guid roleId)
        {
            return new User(Guid.NewGuid(), username, passwordHash, firstName, lastName, roleId);
        }

        /// <summary>
        /// Updates the user's password hash and related flags.
        /// </summary>
        /// <param name="newPasswordHash">The new BCrypt hash.</param>
        /// <param name="isTemporary">Indicates if this is a temporary password (e.g., from an admin reset).</param>
        public void UpdatePassword(string newPasswordHash, bool isTemporary)
        {
            PasswordHash = Guard.Against.NullOrWhiteSpace(newPasswordHash, nameof(newPasswordHash));
            IsTemporaryPassword = isTemporary;
            PasswordLastChangedAt = DateTime.UtcNow;
            MarkAsUpdated();
        }

        /// <summary>
        /// Updates the user's role.
        /// </summary>
        /// <param name="newRoleId">The identifier of the new role.</param>
        public void UpdateRole(Guid newRoleId)
        {
            RoleId = Guard.Against.Default(newRoleId, nameof(newRoleId));
            MarkAsUpdated();
        }

        /// <summary>
        /// Updates the user's profile information.
        /// </summary>
        /// <param name="firstName">The updated first name.</param>
        /// <param name="lastName">The updated last name.</param>
        public void UpdateProfile(string firstName, string lastName)
        {
            FirstName = Guard.Against.NullOrWhiteSpace(firstName, nameof(firstName));
            LastName = Guard.Against.NullOrWhiteSpace(lastName, nameof(lastName));
            MarkAsUpdated();
        }

        /// <summary>
        /// Deactivates the user account.
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
            MarkAsUpdated();
        }

        /// <summary>
        /// Activates the user account.
        /// </summary>
        public void Activate()
        {
            IsActive = true;
            MarkAsUpdated();
        }
        
        /// <summary>
        /// Marks the user as having a permanent password after a forced change.
        /// </summary>
        public void FinalizePasswordChange()
        {
            IsTemporaryPassword = false;
            MarkAsUpdated();
        }
        
        private void MarkAsUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}