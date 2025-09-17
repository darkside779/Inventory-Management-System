using System.Linq.Expressions;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces;

/// <summary>
/// Generic repository interface for common CRUD operations
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
public interface IGenericRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Get entity by ID
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Entity or null if not found</returns>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all entities
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of entities</returns>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get entities with filtering, ordering, and includes
    /// </summary>
    /// <param name="filter">Filter expression</param>
    /// <param name="orderBy">Order by expression</param>
    /// <param name="includeProperties">Navigation properties to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of entities</returns>
    Task<IEnumerable<T>> GetAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string includeProperties = "",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get entities with pagination
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="filter">Filter expression</param>
    /// <param name="orderBy">Order by expression</param>
    /// <param name="includeProperties">Navigation properties to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated collection of entities</returns>
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string includeProperties = "",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get first entity matching the filter
    /// </summary>
    /// <param name="filter">Filter expression</param>
    /// <param name="includeProperties">Navigation properties to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>First entity or null</returns>
    Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>> filter,
        string includeProperties = "",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if entity exists
    /// </summary>
    /// <param name="filter">Filter expression</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if entity exists</returns>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get count of entities matching filter
    /// </summary>
    /// <param name="filter">Filter expression</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Count of entities</returns>
    Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add new entity
    /// </summary>
    /// <param name="entity">Entity to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Added entity</returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add multiple entities
    /// </summary>
    /// <param name="entities">Entities to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Added entities</returns>
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update entity
    /// </summary>
    /// <param name="entity">Entity to update</param>
    /// <returns>Updated entity</returns>
    T Update(T entity);

    /// <summary>
    /// Update multiple entities
    /// </summary>
    /// <param name="entities">Entities to update</param>
    /// <returns>Updated entities</returns>
    IEnumerable<T> UpdateRange(IEnumerable<T> entities);

    /// <summary>
    /// Delete entity by ID
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if deleted</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete entity
    /// </summary>
    /// <param name="entity">Entity to delete</param>
    /// <returns>Deleted entity</returns>
    T Delete(T entity);

    /// <summary>
    /// Delete multiple entities
    /// </summary>
    /// <param name="entities">Entities to delete</param>
    /// <returns>Deleted entities</returns>
    IEnumerable<T> DeleteRange(IEnumerable<T> entities);

    /// <summary>
    /// Soft delete entity by setting IsActive to false
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if soft deleted</returns>
    Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
}
