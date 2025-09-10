// DMPS.Shared.Core/Application/Contracts/Repositories/IUnitOfWork.cs
namespace DMPS.Shared.Core.Application.Contracts.Repositories
{
    /// <summary>
    /// Represents the Unit of Work pattern, which maintains a list of objects affected by a business transaction
    /// and coordinates the writing out of changes and the resolution of concurrency problems.
    /// This contract ensures that multiple repository operations can be committed or rolled back as a single atomic unit.
    /// </summary>
    public interface IUnitOfWork : IAsyncDisposable
    {
        /// <summary>
        /// Asynchronously saves all changes made in this unit of work to the underlying database.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous save operation. The task result contains the number of
        /// state entries written to the database.
        /// </returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}