using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// Supplier repository implementation with specific business operations
/// </summary>
public class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
{
    public SupplierRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get supplier by name
    /// </summary>
    public async Task<Supplier?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Name == name, cancellationToken);
    }

    /// <summary>
    /// Search suppliers by name or contact info
    /// </summary>
    public async Task<IEnumerable<Supplier>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        
        return await _dbSet
            .Where(s => s.IsActive && (
                s.Name.ToLower().Contains(lowerSearchTerm) ||
                (s.ContactInfo != null && s.ContactInfo.ToLower().Contains(lowerSearchTerm)) ||
                (s.Email != null && s.Email.ToLower().Contains(lowerSearchTerm))
            ))
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get suppliers with product counts
    /// </summary>
    public async Task<IEnumerable<(Supplier Supplier, int ProductCount)>> GetSuppliersWithProductCountAsync(CancellationToken cancellationToken = default)
    {
        var query = from s in _dbSet
                    where s.IsActive
                    select new
                    {
                        Supplier = s,
                        ProductCount = s.Products.Count(p => p.IsActive)
                    };

        var results = await query
            .OrderBy(x => x.Supplier.Name)
            .ToListAsync(cancellationToken);

        return results.Select(r => (r.Supplier, r.ProductCount));
    }

    /// <summary>
    /// Check if supplier name is unique
    /// </summary>
    public async Task<bool> IsNameUniqueAsync(string name, int? excludeSupplierId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(s => s.Name == name);
        
        if (excludeSupplierId.HasValue)
        {
            query = query.Where(s => s.Id != excludeSupplierId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Get active suppliers only
    /// </summary>
    public async Task<IEnumerable<Supplier>> GetActiveSuppliersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }
}
