using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// Inventory repository implementation with specific business operations
/// </summary>
public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
{
    public InventoryRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get inventory by product and warehouse
    /// </summary>
    public async Task<Inventory?> GetByProductAndWarehouseAsync(int productId, int warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .FirstOrDefaultAsync(i => i.ProductId == productId && i.WarehouseId == warehouseId, cancellationToken);
    }

    /// <summary>
    /// Get all inventory for a product across warehouses
    /// </summary>
    public async Task<IEnumerable<Inventory>> GetByProductAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.ProductId == productId)
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .OrderBy(i => i.Warehouse.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get all inventory for a warehouse
    /// </summary>
    public async Task<IEnumerable<Inventory>> GetByWarehouseAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.WarehouseId == warehouseId)
            .Include(i => i.Product)
            .ThenInclude(p => p.Category)
            .Include(i => i.Warehouse)
            .OrderBy(i => i.Product.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get inventory items with low stock
    /// </summary>
    public async Task<IEnumerable<Inventory>> GetLowStockItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.Product)
            .ThenInclude(p => p.Category)
            .Include(i => i.Warehouse)
            .Where(i => i.Quantity <= i.Product.LowStockThreshold)
            .OrderBy(i => i.Product.Name)
            .ThenBy(i => i.Warehouse.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get inventory items with available stock
    /// </summary>
    public async Task<IEnumerable<Inventory>> GetAvailableStockAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .Where(i => i.Quantity > i.ReservedQuantity)
            .OrderBy(i => i.Product.Name)
            .ThenBy(i => i.Warehouse.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get total quantity for a product across all warehouses
    /// </summary>
    public async Task<int> GetTotalQuantityByProductAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.ProductId == productId)
            .SumAsync(i => i.Quantity, cancellationToken);
    }

    /// <summary>
    /// Get total available quantity for a product across all warehouses
    /// </summary>
    public async Task<int> GetTotalAvailableQuantityByProductAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.ProductId == productId)
            .SumAsync(i => i.Quantity - i.ReservedQuantity, cancellationToken);
    }

    /// <summary>
    /// Get inventory value summary by warehouse
    /// </summary>
    public async Task<decimal> GetInventoryValueByWarehouseAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.WarehouseId == warehouseId)
            .Include(i => i.Product)
            .SumAsync(i => i.Quantity * (i.Product.Cost ?? 0), cancellationToken);
    }

    /// <summary>
    /// Get overall inventory value across all warehouses
    /// </summary>
    public async Task<decimal> GetTotalInventoryValueAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.Product)
            .SumAsync(i => i.Quantity * (i.Product.Cost ?? 0), cancellationToken);
    }

    /// <summary>
    /// Reserve quantity for a product in a specific warehouse
    /// </summary>
    public async Task<bool> ReserveQuantityAsync(int productId, int warehouseId, int quantity, CancellationToken cancellationToken = default)
    {
        var inventory = await GetByProductAndWarehouseAsync(productId, warehouseId, cancellationToken);
        
        if (inventory == null || !inventory.HasSufficientStock(quantity))
        {
            return false;
        }

        return inventory.ReserveQuantity(quantity);
    }

    /// <summary>
    /// Release reserved quantity for a product in a specific warehouse
    /// </summary>
    public async Task<bool> ReleaseReservedQuantityAsync(int productId, int warehouseId, int quantity, CancellationToken cancellationToken = default)
    {
        var inventory = await GetByProductAndWarehouseAsync(productId, warehouseId, cancellationToken);
        
        if (inventory == null)
        {
            return false;
        }

        return inventory.ReleaseReservedQuantity(quantity);
    }
}
