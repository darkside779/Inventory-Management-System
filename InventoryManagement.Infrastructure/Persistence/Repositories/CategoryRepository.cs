using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// Category repository implementation with specific business operations
/// </summary>
public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get category by name
    /// </summary>
    public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }

    /// <summary>
    /// Get categories with product counts
    /// </summary>
    public async Task<IEnumerable<(Category Category, int ProductCount)>> GetCategoriesWithProductCountAsync(CancellationToken cancellationToken = default)
    {
        var query = from c in _dbSet
                    where c.IsActive
                    select new
                    {
                        Category = c,
                        ProductCount = c.Products.Count(p => p.IsActive)
                    };

        var results = await query
            .OrderBy(x => x.Category.Name)
            .ToListAsync(cancellationToken);

        return results.Select(r => (r.Category, r.ProductCount));
    }

    /// <summary>
    /// Check if category name is unique
    /// </summary>
    public async Task<bool> IsNameUniqueAsync(string name, int? excludeCategoryId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(c => c.Name == name);
        
        if (excludeCategoryId.HasValue)
        {
            query = query.Where(c => c.Id != excludeCategoryId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Get active categories only
    /// </summary>
    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }
}
