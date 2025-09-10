// DMPS.Shared.Core/Domain/Primitives/IAggregateRoot.cs
namespace DMPS.Shared.Core.Domain.Primitives
{
    /// <summary>
    /// A marker interface for an Aggregate Root in the context of Domain-Driven Design.
    /// An Aggregate Root is the primary entity within an aggregate, and it is the only
    /// object that external objects are allowed to hold a reference to.
    /// It is responsible for maintaining the integrity and invariants of the aggregate.
    /// </summary>
    public interface IAggregateRoot
    {
    }
}