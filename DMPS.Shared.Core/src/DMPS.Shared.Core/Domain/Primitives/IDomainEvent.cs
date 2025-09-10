// DMPS.Shared.Core/Domain/Primitives/IDomainEvent.cs
namespace DMPS.Shared.Core.Domain.Primitives
{
    /// <summary>
    /// A marker interface for a Domain Event.
    /// Domain Events represent something that has happened in the domain that other
    /// parts of the same domain (or other domains) might be interested in.
    /// </summary>
    public interface IDomainEvent
    {
    }
}