using MediatR;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Features.Inventory.Queries.GetInventoryById;

/// <summary>
/// Query to retrieve a specific inventory record by ID
/// </summary>
public class GetInventoryByIdQuery : IRequest<InventoryDto?>
{
    /// <summary>
    /// Inventory ID to retrieve
    /// </summary>
    public int Id { get; set; }

    public GetInventoryByIdQuery(int id)
    {
        Id = id;
    }
}

/// <summary>
/// Handler for GetInventoryByIdQuery
/// </summary>
public class GetInventoryByIdQueryHandler : IRequestHandler<GetInventoryByIdQuery, InventoryDto?>
{
    private readonly IApplicationDbContext _context;

    public GetInventoryByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryDto?> Handle(GetInventoryByIdQuery request, CancellationToken cancellationToken)
    {
        var inventory = await _context.Inventories
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        if (inventory == null)
            return null;

        return new InventoryDto
        {
            Id = inventory.Id,
            ProductId = inventory.ProductId,
            ProductName = inventory.Product.Name,
            ProductSKU = inventory.Product.SKU,
            WarehouseId = inventory.WarehouseId,
            WarehouseName = inventory.Warehouse.Name,
            Quantity = inventory.Quantity,
            ReservedQuantity = inventory.ReservedQuantity,
            MinimumStockLevel = inventory.Product.LowStockThreshold,
            MaximumStockLevel = 1000, // Default or from business rules
            Location = inventory.Warehouse.Location,
            IsActive = inventory.IsActive,
            CreatedAt = inventory.CreatedAt,
            UpdatedAt = inventory.UpdatedAt,
            ProductPrice = inventory.Product.Price
        };
    }
}
