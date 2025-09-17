using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces;

/// <summary>
/// Repository interface for Warehouse entity with specific business operations
/// </summary>
public interface IWarehouseRepository : IGenericRepository<Warehouse>
{
    /// <summary>
    /// Get warehouse by name
    /// </summary>
    /// <param name="name">Warehouse name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Warehouse or null</returns>
    Task<Warehouse?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get warehouses by location
    /// </summary>
    /// <param name="location">Location</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Warehouses in the location</returns>
    Task<IEnumerable<Warehouse>> GetByLocationAsync(string location, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get warehouses with inventory summaries
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Warehouses with inventory data</returns>
    Task<IEnumerable<(Warehouse Warehouse, int TotalProducts, int TotalQuantity, decimal TotalValue)>> GetWarehousesWithInventorySummaryAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if warehouse name is unique
    /// </summary>
    /// <param name="name">Warehouse name</param>
    /// <param name="excludeWarehouseId">Warehouse ID to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if name is unique</returns>
    Task<bool> IsNameUniqueAsync(string name, int? excludeWarehouseId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active warehouses only
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Active warehouses</returns>
    Task<IEnumerable<Warehouse>> GetActiveWarehousesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get warehouse capacity utilization
    /// </summary>
    /// <param name="warehouseId">Warehouse ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Capacity utilization percentage (null if no capacity limit)</returns>
    Task<decimal?> GetCapacityUtilizationAsync(int warehouseId, CancellationToken cancellationToken = default);
}
