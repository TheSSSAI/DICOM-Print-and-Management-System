using DMPS.Shared.Core.Entities;
using DMPS.Shared.Core.Repositories;
using DMPS.Data.Access.Contexts;
using Microsoft.EntityFrameworkCore;
using DMPS.Shared.Core.Exceptions;
using Npgsql;

namespace DMPS.Data.Access.Repositories;

public sealed class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<User?> GetByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return null;
        }

        try
        {
            // Eagerly load the Role to prevent N+1 queries later.
            // AsNoTracking is used because this is a read-only operation for authentication.
            return await _dbSet
                .AsNoTracking()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }
        catch (NpgsqlException ex)
        {
            // Abstract the database-specific exception into a generic data access exception.
            throw new DataAccessException($"An error occurred while retrieving the user with username '{username}'.", ex);
        }
    }
    
    /// <inheritdoc />
    public async Task<IReadOnlyList<PasswordHistory>> GetPasswordHistoryAsync(Guid userId, int limit)
    {
        try
        {
            return await _context.PasswordHistories
                .AsNoTracking()
                .Where(ph => ph.UserId == userId)
                .OrderByDescending(ph => ph.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }
        catch (NpgsqlException ex)
        {
            throw new DataAccessException($"An error occurred while retrieving password history for user ID '{userId}'.", ex);
        }
    }
    
    /// <inheritdoc />
    public async Task AddPasswordHistoryAsync(PasswordHistory passwordHistory)
    {
        if (passwordHistory == null) throw new ArgumentNullException(nameof(passwordHistory));

        try
        {
            await _context.PasswordHistories.AddAsync(passwordHistory);
        }
        catch (DbUpdateException ex)
        {
            throw new DataAccessException("An error occurred while adding a password history record.", ex);
        }
    }

    /// <inheritdoc />
    public async Task<User?> GetUserWithRoleByIdAsync(Guid userId)
    {
        try
        {
            return await _dbSet
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }
        catch (NpgsqlException ex)
        {
            throw new DataAccessException($"An error occurred while retrieving the user with ID '{userId}' and their role.", ex);
        }
    }
}