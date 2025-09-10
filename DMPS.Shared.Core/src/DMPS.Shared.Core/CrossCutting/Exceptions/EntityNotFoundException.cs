namespace DMPS.Shared.Core.CrossCutting.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a requested entity cannot be found in the data store.
/// </summary>
public sealed class EntityNotFoundException : DomainException
{
    /// <summary>
    /// Gets the name of the entity that was not found.
    /// </summary>
    public string EntityName { get; }

    /// <summary>
    /// Gets the identifier of the entity that was not found.
    /// </summary>
    public object EntityId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class with a specified entity name and identifier.
    /// </summary>
    /// <param name="entityName">The name of the entity type.</param>
    /// <param name="entityId">The identifier of the entity.</param>
    public EntityNotFoundException(string entityName, object entityId)
        : base($"Entity \"{entityName}\" with ID \"{entityId}\" was not found.")
    {
        EntityName = entityName;
        EntityId = entityId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class with a specified entity name, identifier, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="entityName">The name of the entity type.</param>
    /// <param name="entityId">The identifier of the entity.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public EntityNotFoundException(string entityName, object entityId, Exception innerException)
        : base($"Entity \"{entityName}\" with ID \"{entityId}\" was not found.", innerException)
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}