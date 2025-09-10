using DMPS.Shared.Core.Domain.Primitives;

namespace DMPS.Shared.Core.Domain.Entities;

/// <summary>
/// Represents a single entry in the system's audit trail, recording a significant action.
/// This entity corresponds to the 'AuditLog' table in the database.
/// Fulfills requirements REQ-1-047, REQ-1-048, REQ-1-049.
/// </summary>
public sealed class AuditLog : Entity<long>
{
    /// <summary>
    /// Gets the ID of the user who performed the action. Can be null for system-initiated events.
    /// </summary>
    public Guid? UserId { get; private set; }

    /// <summary>
    /// Gets the timestamp when the event occurred.
    /// </summary>
    public DateTime EventTimestamp { get; private set; }

    /// <summary>
    /// Gets the type of event that occurred (e.g., 'UserLogin', 'DicomEdit', 'StudyDeleted').
    /// </summary>
    public string EventType { get; private set; }

    /// <summary>
    /// Gets the name of the entity that was affected by the event (e.g., 'User', 'Study').
    /// </summary>
    public string? EntityName { get; private set; }

    /// <summary>
    /// Gets the primary key or unique identifier of the affected entity.
    /// </summary>
    public string? EntityId { get; private set; }

    /// <summary>
    /// Gets a JSON object containing event-specific details, such as old and new values for a modification.
    /// </summary>
    public string? Details { get; private set; }

    /// <summary>
    /// Gets the ID used to trace a single operation across multiple system components.
    /// </summary>
    public Guid? CorrelationId { get; private set; }
    
    /// <summary>
    /// Private constructor for ORM materialization.
    /// </summary>
    private AuditLog() : base(0) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditLog"/> class.
    /// </summary>
    private AuditLog(Guid? userId, DateTime eventTimestamp, string eventType, string? entityName, string? entityId, string? details, Guid? correlationId) : base(0)
    {
        UserId = userId;
        EventTimestamp = eventTimestamp;
        EventType = eventType;
        EntityName = entityName;
        EntityId = entityId;
        Details = details;
        CorrelationId = correlationId;
    }

    /// <summary>
    /// Creates a new audit log entry.
    /// </summary>
    /// <param name="userId">The ID of the user performing the action. Null for system events.</param>
    /// <param name="eventType">The type of event.</param>
    /// <param name="entityName">Optional name of the affected entity.</param>
    /// <param name="entityId">Optional ID of the affected entity.</param>
    /// <param name="details">Optional JSON string with event details.</param>
    /// <param name="correlationId">Optional correlation ID for tracing.</param>
    /// <returns>A new <see cref="AuditLog"/> instance.</returns>
    public static AuditLog Create(Guid? userId, string eventType, string? entityName = null, string? entityId = null, string? details = null, Guid? correlationId = null)
    {
        Common.Guard.Against.NullOrWhiteSpace(eventType, nameof(eventType));

        return new AuditLog(
            userId, 
            DateTime.UtcNow, 
            eventType, 
            entityName, 
            entityId, 
            details, 
            correlationId
        );
    }
}