using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// Generic repository implementation for common CRUD operations
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<T>();
    }

    /// <summary>
    /// Get entity by ID
    /// </summary>
    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    /// <summary>
    /// Get all entities
    /// </summary>
    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get entities with filtering, ordering, and includes
    /// </summary>
    public virtual async Task<IEnumerable<T>> GetAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string includeProperties = "",
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        foreach (var includeProperty in includeProperties.Split(
            new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty.Trim());
        }

        if (orderBy != null)
        {
            return await orderBy(query).ToListAsync(cancellationToken);
        }
        else
        {
            return await query.ToListAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Get entities with pagination
    /// </summary>
    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string includeProperties = "",
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        foreach (var includeProperty in includeProperties.Split(
            new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty.Trim());
        }

        var totalCount = await query.CountAsync(cancellationToken);

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <summary>
    /// Get first entity matching the filter
    /// </summary>
    public virtual async Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>> filter,
        string includeProperties = "",
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;

        foreach (var includeProperty in includeProperties.Split(
            new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty.Trim());
        }

        return await query.FirstOrDefaultAsync(filter, cancellationToken);
    }

    /// <summary>
    /// Check if entity exists
    /// </summary>
    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(filter, cancellationToken);
    }

    /// <summary>
    /// Get count of entities matching filter
    /// </summary>
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        if (filter != null)
        {
            return await _dbSet.CountAsync(filter, cancellationToken);
        }
        else
        {
            return await _dbSet.CountAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Add new entity
    /// </summary>
    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    /// <summary>
    /// Add multiple entities
    /// </summary>
    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    /// <summary>
    /// Update entity
    /// </summary>
    public virtual T Update(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return entity;
    }

    /// <summary>
    /// Update multiple entities
    /// </summary>
    public virtual IEnumerable<T> UpdateRange(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
        return entities;
    }

    /// <summary>
    /// Delete entity by ID
    /// </summary>
    public virtual async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity == null)
        {
            return false;
        }

        Delete(entity);
        return true;
    }

    /// <summary>
    /// Delete entity
    /// </summary>
    public virtual T Delete(T entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }
        _dbSet.Remove(entity);
        return entity;
    }

    /// <summary>
    /// Delete multiple entities
    /// </summary>
    public virtual IEnumerable<T> DeleteRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
        return entities;
    }

    /// <summary>
    /// Soft delete entity by setting IsActive to false
    /// </summary>
    public virtual async Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity == null)
        {
            return false;
        }

        entity.IsActive = false;
        Update(entity);
        return true;
    }
}
