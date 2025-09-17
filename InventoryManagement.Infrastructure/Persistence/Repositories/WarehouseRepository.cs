using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// Warehouse repository implementation with specific business operations
/// </summary>
public class WarehouseRepository : GenericRepository<Warehouse>, IWarehouseRepository
{
    public WarehouseRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get warehouse by name
    /// </summary>
    public async Task<Warehouse?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(w => w.Name == name, cancellationToken);
    }

    /// <summary>
    /// Get warehouses by location
    /// </summary>
    public async Task<IEnumerable<Warehouse>> GetByLocationAsync(string location, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(w => w.Location.Contains(location) && w.IsActive)
            .OrderBy(w => w.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get warehouses with inventory summaries
    /// </summary>
    public async Task<IEnumerable<(Warehouse Warehouse, int TotalProducts, int TotalQuantity, decimal TotalValue)>> GetWarehousesWithInventorySummaryAsync(CancellationToken cancellationToken = default)
    {
        var query = from w in _dbSet
                    where w.IsActive
                    select new
                    {
                        Warehouse = w,
                        TotalProducts = w.InventoryItems.Count(),
                        TotalQuantity = w.InventoryItems.Sum(i => i.Quantity),
                        TotalValue = w.InventoryItems.Sum(i => i.Quantity * (i.Product.Cost ?? 0))
                    };

        var results = await query
            .OrderBy(x => x.Warehouse.Name)
            .ToListAsync(cancellationToken);

        return results.Select(r => (r.Warehouse, r.TotalProducts, r.TotalQuantity, r.TotalValue));
    }

    /// <summary>
    /// Check if warehouse name is unique
    /// </summary>
    public async Task<bool> IsNameUniqueAsync(string name, int? excludeWarehouseId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(w => w.Name == name);
        
        if (excludeWarehouseId.HasValue)
        {
            query = query.Where(w => w.Id != excludeWarehouseId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Get active warehouses only
    /// </summary>
    public async Task<IEnumerable<Warehouse>> GetActiveWarehousesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(w => w.IsActive)
            .OrderBy(w => w.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get warehouse capacity utilization
    /// </summary>
    public async Task<decimal?> GetCapacityUtilizationAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        var warehouse = await _dbSet
            .Include(w => w.InventoryItems)
            .FirstOrDefaultAsync(w => w.Id == warehouseId, cancellationToken);

        if (warehouse?.Capacity == null || warehouse.Capacity <= 0)
        {
            return null; // No capacity limit set
        }

        var currentQuantity = warehouse.InventoryItems.Sum(i => i.Quantity);
        return (decimal)currentQuantity / warehouse.Capacity.Value * 100;
    }
}
