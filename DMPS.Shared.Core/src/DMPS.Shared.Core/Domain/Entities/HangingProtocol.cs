using DMPS.Shared.Core.Common;
using DMPS.Shared.Core.Domain.Primitives;

namespace DMPS.Shared.Core.Domain.Entities;

/// <summary>
/// Represents a user-defined or system-wide display layout (hanging protocol).
/// </summary>
public sealed class HangingProtocol : Entity<Guid>, IAggregateRoot
{
    /// <summary>
    /// The name of the hanging protocol.
    /// </summary>
    public string ProtocolName { get; private set; } = string.Empty;

    /// <summary>
    /// The ID of the user who owns this protocol. Null for system-wide protocols.
    /// </summary>
    public Guid? UserId { get; private set; }

    /// <summary>
    /// A JSON object defining the viewport layout (e.g., {"grid": "2x2"}).
    /// </summary>
    public string LayoutDefinition { get; private set; } = string.Empty;

    /// <summary>
    /// A JSON object with matching criteria for auto-application 
    /// (e.g., {"Modality": "CT", "BodyPartExamined": "CHEST"}).
    /// </summary>
    public string? Criteria { get; private set; }

    /// <summary>
    /// The timestamp when the protocol was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// The timestamp of the last update to the protocol.
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Navigation property to the user who owns this protocol.
    /// </summary>
    public User? User { get; private set; }

    /// <summary>
    /// Private constructor for Entity Framework Core.
    /// </summary>
    private HangingProtocol() : base(Guid.NewGuid()) { }

    /// <summary>
    /// Creates a new instance of the <see cref="HangingProtocol"/> entity.
    /// </summary>
    /// <param name="id">The unique identifier for the hanging protocol.</param>
    /// <param name="protocolName">The name of the protocol.</param>
    /// <param name="userId">The ID of the owner user, or null for a system protocol.</param>
    /// <param name="layoutDefinition">The JSON definition of the layout.</param>
    /// <param name="criteria">The JSON definition of the auto-apply criteria.</param>
    /// <returns>A new <see cref="HangingProtocol"/> instance.</returns>
    public static HangingProtocol Create(Guid id, string protocolName, Guid? userId, string layoutDefinition, string? criteria)
    {
        Guard.Against.Default(id, nameof(id));
        Guard.Against.NullOrWhiteSpace(protocolName, nameof(protocolName));
        Guard.Against.NullOrWhiteSpace(layoutDefinition, nameof(layoutDefinition));

        var now = DateTime.UtcNow;
        return new HangingProtocol
        {
            Id = id,
            ProtocolName = protocolName,
            UserId = userId,
            LayoutDefinition = layoutDefinition,
            Criteria = criteria,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    /// <summary>
    /// Updates the properties of the hanging protocol.
    /// </summary>
    /// <param name="protocolName">The new name for the protocol.</param>
    /// <param name="layoutDefinition">The new layout definition.</param>
    /// <param name="criteria">The new auto-apply criteria.</param>
    public void Update(string protocolName, string layoutDefinition, string? criteria)
    {
        Guard.Against.NullOrWhiteSpace(protocolName, nameof(protocolName));
        Guard.Against.NullOrWhiteSpace(layoutDefinition, nameof(layoutDefinition));

        ProtocolName = protocolName;
        LayoutDefinition = layoutDefinition;
        Criteria = criteria;
        UpdatedAt = DateTime.UtcNow;
    }
}