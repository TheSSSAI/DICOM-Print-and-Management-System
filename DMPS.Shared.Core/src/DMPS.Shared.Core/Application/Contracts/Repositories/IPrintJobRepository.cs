using DMPS.Shared.Core.Domain.Entities;

namespace DMPS.Shared.Core.Application.Contracts.Repositories;

/// <summary>
/// Defines the contract for a repository that handles data operations for PrintJob entities.
/// </summary>
public interface IPrintJobRepository : IGenericRepository<PrintJob, Guid>
{
    /// <summary>
    /// Asynchronously retrieves a paged list of print jobs for display in the print queue UI.
    /// This method is intended for Administrators who can view all jobs.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of jobs per page.</param>
    /// <param name="includeCompleted">A flag to indicate whether to include completed and canceled jobs.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of print jobs.</returns>
    Task<IEnumerable<PrintJob>> GetPrintQueueAsync(int pageNumber, int pageSize, bool includeCompleted);

    /// <summary>
    /// Asynchronously retrieves all active (queued or processing) print jobs submitted by a specific user.
    /// This method is intended for Technicians to view their own jobs.
    /// </summary>
    /// <param name="userId">The unique identifier of the user who submitted the jobs.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of the user's active print jobs.</returns>
    Task<IEnumerable<PrintJob>> GetActivePrintJobsByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Asynchronously gets the total count of print jobs based on a filter.
    /// </summary>
    /// <param name="includeCompleted">A flag to indicate whether to include completed and canceled jobs.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the total count of jobs.</returns>
    Task<int> GetTotalCountAsync(bool includeCompleted);
}