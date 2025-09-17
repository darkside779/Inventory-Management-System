using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces;

/// <summary>
/// Repository interface for Category entity with specific business operations
/// </summary>
public interface ICategoryRepository : IGenericRepository<Category>
{
    /// <summary>
    /// Get category by name
    /// </summary>
    /// <param name="name">Category name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Category or null</returns>
    Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get categories with product counts
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Categories with product counts</returns>
    Task<IEnumerable<(Category Category, int ProductCount)>> GetCategoriesWithProductCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if category name is unique
    /// </summary>
    /// <param name="name">Category name</param>
    /// <param name="excludeCategoryId">Category ID to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if name is unique</returns>
    Task<bool> IsNameUniqueAsync(string name, int? excludeCategoryId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active categories only
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Active categories</returns>
    Task<IEnumerable<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);
}
