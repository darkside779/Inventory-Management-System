using MediatR;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Features.Warehouses.Queries.GetWarehouseWithInventory;

/// <summary>
/// Query to retrieve a warehouse with detailed inventory information
/// </summary>
public class GetWarehouseWithInventoryQuery : IRequest<GetWarehouseWithInventoryResponse?>
{
    /// <summary>
    /// Warehouse ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Include only active inventory items
    /// </summary>
    public bool ActiveOnly { get; set; } = true;

    /// <summary>
    /// Filter by low stock items only
    /// </summary>
    public bool LowStockOnly { get; set; } = false;
}

/// <summary>
/// Response containing warehouse details with inventory information
/// </summary>
public class GetWarehouseWithInventoryResponse
{
    /// <summary>
    /// Warehouse details
    /// </summary>
    public WarehouseDto Warehouse { get; set; } = new();

    /// <summary>
    /// Inventory items in this warehouse
    /// </summary>
    public List<InventoryDto> InventoryItems { get; set; } = new();

    /// <summary>
    /// Total number of different products
    /// </summary>
    public int TotalProducts { get; set; }

    /// <summary>
    /// Total quantity of all items
    /// </summary>
    public int TotalQuantity { get; set; }

    /// <summary>
    /// Total value of all inventory
    /// </summary>
    public decimal TotalValue { get; set; }

    /// <summary>
    /// Number of low stock items
    /// </summary>
    public int LowStockItemsCount { get; set; }

    /// <summary>
    /// Current utilization percentage
    /// </summary>
    public decimal? UtilizationPercentage { get; set; }
}

/// <summary>
/// Handler for GetWarehouseWithInventoryQuery
/// </summary>
public class GetWarehouseWithInventoryQueryHandler : IRequestHandler<GetWarehouseWithInventoryQuery, GetWarehouseWithInventoryResponse?>
{
    private readonly IApplicationDbContext _context;

    public GetWarehouseWithInventoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetWarehouseWithInventoryResponse?> Handle(GetWarehouseWithInventoryQuery request, CancellationToken cancellationToken)
    {
        // Get warehouse details
        var warehouse = await _context.Warehouses
            .Where(w => w.Id == request.Id)
            .Select(w => new WarehouseDto
            {
                Id = w.Id,
                Name = w.Name,
                Location = w.Location,
                Description = w.Address,
                Capacity = w.Capacity,
                IsActive = w.IsActive,
                CreatedAt = w.CreatedAt,
                UpdatedAt = w.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (warehouse == null)
        {
            return null;
        }

        // Get inventory items query
        var inventoryQuery = _context.Inventories
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .Where(i => i.WarehouseId == request.Id);

        // Apply filters
        if (request.ActiveOnly)
        {
            inventoryQuery = inventoryQuery.Where(i => i.IsActive);
        }

        if (request.LowStockOnly)
        {
            inventoryQuery = inventoryQuery.Where(i => i.Quantity <= i.Product.LowStockThreshold);
        }

        // Get inventory items
        var inventoryItems = await inventoryQuery
            .Select(i => new InventoryDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                ProductSKU = i.Product.SKU,
                WarehouseId = i.WarehouseId,
                WarehouseName = i.Warehouse.Name,
                Quantity = i.Quantity,
                ReservedQuantity = i.ReservedQuantity,
                MinimumStockLevel = i.Product.LowStockThreshold,
                MaximumStockLevel = 1000, // Default or from business rules
                Location = i.Warehouse.Location,
                IsActive = i.IsActive,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
                ProductPrice = i.Product.Price
            })
            .ToListAsync(cancellationToken);

        // Calculate summary statistics
        var totalProducts = inventoryItems.Count;
        var totalQuantity = inventoryItems.Sum(i => i.Quantity);
        var totalValue = inventoryItems.Sum(i => i.Quantity * i.ProductPrice);
        var lowStockItemsCount = inventoryItems.Count(i => i.Quantity <= i.MinimumStockLevel);

        // Calculate utilization percentage
        decimal? utilizationPercentage = null;
        if (warehouse.Capacity.HasValue && warehouse.Capacity.Value > 0)
        {
            utilizationPercentage = (decimal)totalQuantity / warehouse.Capacity.Value * 100;
        }

        // Update warehouse summary info
        warehouse.InventoryItemCount = totalProducts;
        warehouse.CapacityUtilization = utilizationPercentage;

        return new GetWarehouseWithInventoryResponse
        {
            Warehouse = warehouse,
            InventoryItems = inventoryItems,
            TotalProducts = totalProducts,
            TotalQuantity = totalQuantity,
            TotalValue = totalValue,
            LowStockItemsCount = lowStockItemsCount,
            UtilizationPercentage = utilizationPercentage
        };
    }
}
