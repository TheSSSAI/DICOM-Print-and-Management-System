using DMPS.Shared.Core.Entities;
using DMPS.Shared.Core.Exceptions;
using DMPS.Shared.Core.Repositories;
using DMPS.Data.Access.Contexts;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Linq.Expressions;

namespace DMPS.Data.Access.Repositories;

/// <summary>
/// A generic repository implementation providing common data access operations for any entity.
/// </summary>
/// <typeparam name="TEntity">The type of the entity. Must be a class and inherit from EntityBase.</typeparam>
public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : EntityBase
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericRepository{TEntity}"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public GenericRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<TEntity>();
    }

    /// <inheritdoc />
    public virtual async Task<TEntity?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _dbSet.FindAsync(id);
        }
        catch (NpgsqlException ex)
        {
            throw new DataAccessException("An error occurred while retrieving the entity by its ID.", ex);
        }
    }
    
    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync()
    {
        try
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }
        catch (NpgsqlException ex)
        {
            throw new DataAccessException("An error occurred while retrieving all entities.", ex);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        }
        catch (NpgsqlException ex)
        {
            throw new DataAccessException("An error occurred while finding entities with a predicate.", ex);
        }
    }

    /// <inheritdoc />
    public async Task AddAsync(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        
        try
        {
            await _dbSet.AddAsync(entity);
        }
        catch (DbUpdateException ex)
        {
            throw new DataAccessException("An error occurred while adding the entity.", ex);
        }
    }
    
    /// <inheritdoc />
    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        if (entities == null) throw new ArgumentNullException(nameof(entities));

        try
        {
            await _dbSet.AddRangeAsync(entities);
        }
        catch (DbUpdateException ex)
        {
            throw new DataAccessException("An error occurred while adding a range of entities.", ex);
        }
    }

    /// <inheritdoc />
    public void Update(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        // This method only marks the entity as Modified. 
        // SaveChangesAsync in the Unit of Work is responsible for the actual database operation.
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    /// <inheritdoc />
    public void Delete(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        // This method only marks the entity as Deleted.
        // SaveChangesAsync in the Unit of Work is responsible for the actual database operation.
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }
        _dbSet.Remove(entity);
    }
    
    /// <inheritdoc />
    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            return await _dbSet.AnyAsync(predicate);
        }
        catch (NpgsqlException ex)
        {
            throw new DataAccessException("An error occurred while checking for entity existence.", ex);
        }
    }
}