using DMPS.Shared.Core.Domain.Entities;

namespace DMPS.Shared.Core.Application.Contracts.Repositories;

/// <summary>
/// Defines the contract for a repository that handles data access for <see cref="AutoRoutingRule"/> entities.
/// </summary>
public interface IAutoRoutingRuleRepository : IGenericRepository<AutoRoutingRule>
{
    /// <summary>
    /// Asynchronously retrieves all active auto-routing rules, ordered by priority.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains an ordered collection of active <see cref="AutoRoutingRule"/> entities.
    /// </returns>
    Task<IEnumerable<AutoRoutingRule>> GetActiveRulesOrderedByPriorityAsync();
}