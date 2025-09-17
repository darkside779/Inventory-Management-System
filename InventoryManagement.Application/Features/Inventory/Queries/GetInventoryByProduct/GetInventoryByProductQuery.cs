using MediatR;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Features.Inventory.Queries.GetInventoryByProduct;

/// <summary>
/// Query to retrieve all inventory records for a specific product across all warehouses
/// </summary>
public class GetInventoryByProductQuery : IRequest<IEnumerable<InventoryDto>>
{
    /// <summary>
    /// Product ID to retrieve inventory for
    /// </summary>
    public int ProductId { get; set; }
    
    /// <summary>
    /// Include only active inventory records
    /// </summary>
    public bool ActiveOnly { get; set; } = true;

    public GetInventoryByProductQuery(int productId, bool activeOnly = true)
    {
        ProductId = productId;
        ActiveOnly = activeOnly;
    }
}

/// <summary>
/// Handler for GetInventoryByProductQuery
/// </summary>
public class GetInventoryByProductQueryHandler : IRequestHandler<GetInventoryByProductQuery, IEnumerable<InventoryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetInventoryByProductQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InventoryDto>> Handle(GetInventoryByProductQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Inventories
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .Where(i => i.ProductId == request.ProductId);

        if (request.ActiveOnly)
        {
            query = query.Where(i => i.IsActive);
        }

        var inventories = await query
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
            .OrderBy(i => i.WarehouseName)
            .ToListAsync(cancellationToken);

        return inventories;
    }
}
