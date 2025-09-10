using DMPS.Shared.Core.Entities;
using DMPS.Shared.Core.Repositories;
using DMPS.Shared.Core.Models;
using DMPS.Data.Access.Contexts;
using Microsoft.EntityFrameworkCore;
using DMPS.Shared.Core.Exceptions;
using Npgsql;

namespace DMPS.Data.Access.Repositories;

public sealed class StudyRepository : GenericRepository<Study>, IStudyRepository
{
    public StudyRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<bool> StudyInstanceUidExistsAsync(string studyInstanceUid)
    {
        try
        {
            return await _dbSet.AnyAsync(s => s.StudyInstanceUid == studyInstanceUid);
        }
        catch (NpgsqlException ex)
        {
            throw new DataAccessException($"An error occurred while checking for existence of StudyInstanceUID '{studyInstanceUid}'.", ex);
        }
    }

    /// <inheritdoc />
    public async Task<PagedResult<Study>> FindStudiesAsync(StudySearchCriteria criteria, int pageNumber, int pageSize)
    {
        try
        {
            IQueryable<Study> query = _dbSet
                .AsNoTracking()
                .Include(s => s.Patient)
                .Include(s => s.Series);

            // Dynamically build the query based on provided criteria
            if (!string.IsNullOrWhiteSpace(criteria.PatientName))
            {
                // PatientName is encrypted. This assumes a function or extension method for handling encrypted search.
                // For a simple LIKE, this would be `s.Patient.PatientName.Contains(criteria.PatientName)`
                // With pgcrypto, this is more complex and might require raw SQL or a custom function mapping.
                // As a placeholder for this complex logic, we'll use a direct comparison for demonstration.
                query = query.Where(s => EF.Functions.ILike(s.Patient.PatientName, $"%{criteria.PatientName}%"));
            }

            if (!string.IsNullOrWhiteSpace(criteria.PatientId))
            {
                query = query.Where(s => EF.Functions.ILike(s.Patient.DicomPatientId, $"%{criteria.PatientId}%"));
            }
            
            if (criteria.StudyDateFrom.HasValue)
            {
                query = query.Where(s => s.StudyDate >= criteria.StudyDateFrom.Value.ToUniversalTime());
            }

            if (criteria.StudyDateTo.HasValue)
            {
                var dateTo = criteria.StudyDateTo.Value.ToUniversalTime().Date.AddDays(1).AddTicks(-1);
                query = query.Where(s => s.StudyDate <= dateTo);
            }

            if (!string.IsNullOrWhiteSpace(criteria.Modality))
            {
                query = query.Where(s => s.Series.Any(series => series.Modality == criteria.Modality));
            }
            
            // Get total count for pagination before applying paging
            var totalRecords = await query.CountAsync();
            
            // Apply sorting
            query = query.OrderByDescending(s => s.StudyDate).ThenBy(s => s.Patient.PatientName);

            // Apply pagination
            var results = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Study>(results, pageNumber, pageSize, totalRecords);
        }
        catch (NpgsqlException ex)
        {
            throw new DataAccessException("An error occurred while searching for studies.", ex);
        }
    }
    
    /// <inheritdoc />
    public async Task<Study?> GetStudyWithDetailsAsync(Guid studyId)
    {
        try
        {
            return await _dbSet
                .Include(s => s.Patient)
                .Include(s => s.Series)
                .ThenInclude(series => series.Images)
                .Include(s => s.Series)
                .ThenInclude(series => series.PresentationStates)
                .FirstOrDefaultAsync(s => s.StudyId == studyId);
        }
        catch (NpgsqlException ex)
        {
            throw new DataAccessException($"An error occurred while retrieving detailed study information for ID '{studyId}'.", ex);
        }
    }
}