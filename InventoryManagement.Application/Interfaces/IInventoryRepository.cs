using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces;

/// <summary>
/// Repository interface for Inventory entity with specific business operations
/// </summary>
public interface IInventoryRepository : IGenericRepository<Inventory>
{
    /// <summary>
    /// Get inventory by product and warehouse
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="warehouseId">Warehouse ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Inventory item or null</returns>
    Task<Inventory?> GetByProductAndWarehouseAsync(int productId, int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all inventory for a product across warehouses
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Inventory items for the product</returns>
    Task<IEnumerable<Inventory>> GetByProductAsync(int productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all inventory for a warehouse
    /// </summary>
    /// <param name="warehouseId">Warehouse ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Inventory items in the warehouse</returns>
    Task<IEnumerable<Inventory>> GetByWarehouseAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get inventory items with low stock
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Low stock inventory items</returns>
    Task<IEnumerable<Inventory>> GetLowStockItemsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get inventory items with available stock (quantity > reservedQuantity)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Inventory items with available stock</returns>
    Task<IEnumerable<Inventory>> GetAvailableStockAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get total quantity for a product across all warehouses
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total quantity</returns>
    Task<int> GetTotalQuantityByProductAsync(int productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get total available quantity for a product across all warehouses
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total available quantity</returns>
    Task<int> GetTotalAvailableQuantityByProductAsync(int productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get inventory value summary by warehouse
    /// </summary>
    /// <param name="warehouseId">Warehouse ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total inventory value</returns>
    Task<decimal> GetInventoryValueByWarehouseAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get overall inventory value across all warehouses
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total inventory value</returns>
    Task<decimal> GetTotalInventoryValueAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Reserve quantity for a product in a specific warehouse
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="warehouseId">Warehouse ID</param>
    /// <param name="quantity">Quantity to reserve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if reservation successful</returns>
    Task<bool> ReserveQuantityAsync(int productId, int warehouseId, int quantity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Release reserved quantity for a product in a specific warehouse
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="warehouseId">Warehouse ID</param>
    /// <param name="quantity">Quantity to release</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if release successful</returns>
    Task<bool> ReleaseReservedQuantityAsync(int productId, int warehouseId, int quantity, CancellationToken cancellationToken = default);
}
