using DMPS.Shared.Core.Domain.Entities;

namespace DMPS.Shared.Core.Application.Contracts.Repositories;

/// <summary>
/// Defines the contract for a repository handling data access for User entities.
/// </summary>
public interface IUserRepository : IGenericRepository<User, Guid>
{
    /// <summary>
    /// Asynchronously retrieves a user by their unique username.
    /// The search should be case-insensitive.
    /// </summary>
    /// <param name="username">The username of the user to find.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <see cref="User"/> entity if found; otherwise, null.
    /// </returns>
    Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously checks if a username already exists in the system.
    /// This is an optimized check that should be faster than retrieving the full user entity.
    /// </summary>
    /// <param name="username">The username to check for existence.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result is true if the username exists; otherwise, false.
    /// </returns>
    Task<bool> CheckUsernameExistsAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves the password history for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="count">The number of recent password hashes to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a collection of the user's most recent <see cref="PasswordHistory"/> records.
    /// </returns>
    Task<IEnumerable<PasswordHistory>> GetPasswordHistoryAsync(Guid userId, int count, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Asynchronously adds a new password history record for a user.
    /// </summary>
    /// <param name="passwordHistory">The password history record to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddPasswordHistoryAsync(PasswordHistory passwordHistory, CancellationToken cancellationToken = default);
}