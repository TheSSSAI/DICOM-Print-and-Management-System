using DMPS.Shared.Core.Domain.Primitives;

namespace DMPS.Shared.Core.Domain.Entities;

/// <summary>
/// Represents an active user session, tracking login time and activity.
/// This entity corresponds to the 'UserSession' table in the database.
/// Fulfills requirements REQ-1-019 and REQ-1-041.
/// </summary>
public sealed class UserSession : Entity<Guid>
{
    /// <summary>
    /// Gets the ID of the user associated with this session.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the timestamp when the user logged in to start this session.
    /// </summary>
    public DateTime LoginTimestamp { get; private set; }

    /// <summary>
    /// Gets the timestamp of the user's last detected activity.
    /// </summary>
    public DateTime LastActivityTimestamp { get; private set; }

    /// <summary>
    /// Gets the IP address from which the session was initiated.
    /// </summary>
    public string? IpAddress { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the session is currently active.
    /// A session becomes inactive upon explicit logout or forced termination.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Private constructor for ORM materialization.
    /// </summary>
    private UserSession() : base(Guid.NewGuid()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSession"/> class.
    /// </summary>
    private UserSession(Guid id, Guid userId, string? ipAddress) : base(id)
    {
        UserId = userId;
        LoginTimestamp = DateTime.UtcNow;
        LastActivityTimestamp = LoginTimestamp;
        IpAddress = ipAddress;
        IsActive = true;
    }

    /// <summary>
    /// Creates a new user session.
    /// </summary>
    /// <param name="userId">The ID of the user starting the session.</param>
    /// <param name="ipAddress">The IP address of the client.</param>
    /// <returns>A new <see cref="UserSession"/> instance.</returns>
    public static UserSession Create(Guid userId, string? ipAddress)
    {
        return new UserSession(Guid.NewGuid(), userId, ipAddress);
    }

    /// <summary>
    /// Updates the last activity timestamp for the session.
    /// </summary>
    public void UpdateActivity()
    {
        LastActivityTimestamp = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the session, marking it as terminated.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        LastActivityTimestamp = DateTime.UtcNow;
    }
}