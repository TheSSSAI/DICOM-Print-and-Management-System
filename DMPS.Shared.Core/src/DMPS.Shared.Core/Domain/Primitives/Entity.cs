using DMPS.Shared.Core.Common;

namespace DMPS.Shared.Core.Domain.Primitives;

/// <summary>
/// Represents a base class for entities in the domain model.
/// An entity is an object that is not defined by its attributes, but rather by a thread of continuity and its identity.
/// </summary>
/// <typeparam name="TId">The type of the entity's identifier.</typeparam>
public abstract class Entity<TId> : IEquatable<Entity<TId>> where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity{TId}"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    protected Entity(TId id)
    {
        Guard.Against.Null(id, nameof(id));
        Id = id;
    }

    /// <summary>
    /// Protected constructor for ORM frameworks.
    /// </summary>
    protected Entity()
    {
        // Required for ORM frameworks like Entity Framework Core.
    }

    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    public TId Id { get; protected set; } = default!;

    /// <summary>
    /// Gets the collection of domain events raised by this entity.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event to the entity.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clears all domain events from the entity.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Determines whether two entity instances are equal.
    /// </summary>
    /// <param name="left">The first entity to compare.</param>
    /// <param name="right">The second entity to compare.</param>
    /// <returns>true if the entities are equal; otherwise, false.</returns>
    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            return true;
        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two entity instances are not equal.
    /// </summary>
    /// <param name="left">The first entity to compare.</param>
    /// <param name="right">The second entity to compare.</param>
    /// <returns>true if the entities are not equal; otherwise, false.</returns>
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !(left == right);

    /// <summary>
    /// Determines whether the specified object is equal to the current entity.
    /// Equality is based on the entity's identifier.
    /// </summary>
    /// <param name="obj">The object to compare with the current entity.</param>
    /// <returns>true if the specified object is equal to the current entity; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (obj.GetType() != GetType()) return false;
        if (obj is not Entity<TId> entity) return false;
        return Equals(entity);
    }

    /// <summary>
    /// Indicates whether the current entity is equal to another entity of the same type.
    /// Equality is based on the entity's identifier.
    /// </summary>
    /// <param name="other">An entity to compare with this entity.</param>
    /// <returns>true if the current entity is equal to the other parameter; otherwise, false.</returns>
    public bool Equals(Entity<TId>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    /// <summary>
    /// Returns the hash code for this entity.
    /// The hash code is based on the entity's identifier.
    /// </summary>
    /// <returns>A hash code for the current entity.</returns>
    public override int GetHashCode() => Id.GetHashCode() * 41;
}