using DMPS.Shared.Core.Entities;
using DMPS.Shared.Core.Models;
using DMPS.Shared.Core.Repositories;
using DMPS.Data.Access.Contexts;
using Microsoft.EntityFrameworkCore;
using DMPS.Shared.Core.Exceptions;
using Npgsql;

namespace DMPS.Data.Access.Repositories;

public sealed class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<PagedResult<AuditLog>> GetFilteredAuditLogsAsync(AuditLogFilter filter, int pageNumber, int pageSize)
    {
        if (filter == null) throw new ArgumentNullException(nameof(filter));

        try
        {
            IQueryable<AuditLog> query = _dbSet.AsNoTracking().Include(a => a.User);

            if (filter.UserId.HasValue)
            {
                query = query.Where(log => log.UserId == filter.UserId.Value);
            }

            if (filter.DateFrom.HasValue)
            {
                query = query.Where(log => log.EventTimestamp >= filter.DateFrom.Value.ToUniversalTime());
            }
            
            if (filter.DateTo.HasValue)
            {
                var dateTo = filter.DateTo.Value.ToUniversalTime().Date.AddDays(1).AddTicks(-1);
                query = query.Where(log => log.EventTimestamp <= dateTo);
            }

            if (!string.IsNullOrWhiteSpace(filter.EventType))
            {
                query = query.Where(log => log.EventType == filter.EventType);
            }

            if (filter.CorrelationId.HasValue)
            {
                query = query.Where(log => log.CorrelationId == filter.CorrelationId.Value);
            }
            
            // Get total count for pagination
            var totalRecords = await query.CountAsync();
            
            // Apply sorting and pagination
            var results = await query
                .OrderByDescending(log => log.EventTimestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<AuditLog>(results, pageNumber, pageSize, totalRecords);
        }
        catch (NpgsqlException ex)
        {
            throw new DataAccessException("An error occurred while retrieving filtered audit logs.", ex);
        }
    }
}