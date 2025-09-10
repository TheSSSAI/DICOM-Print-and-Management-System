using DMPS.Shared.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using DMPS.Shared.Core.Application.Contracts.DTOs;

namespace DMPS.Shared.Core.Application.Contracts.Repositories
{
    /// <summary>
    /// Defines the contract for data access operations related to AuditLog entities.
    /// This repository provides specialized querying capabilities beyond basic CRUD operations.
    /// Fulfills requirement FR-3.4.2.3 for viewing and filtering the audit trail.
    /// </summary>
    public interface IAuditLogRepository : IGenericRepository<AuditLog, long>
    {
        /// <summary>
        /// Finds audit log entries that match the specified filter criteria.
        /// </summary>
        /// <param name="criteria">The criteria to filter the audit logs by, such as user and date range.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a collection of <see cref="AuditLog"/> entities that match the criteria.
        /// </returns>
        Task<IEnumerable<AuditLog>> FindAsync(AuditFilterCriteria criteria, CancellationToken cancellationToken = default);
    }
}