using MediatR;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Features.Warehouses.Queries.GetWarehouseById;

/// <summary>
/// Query to retrieve a specific warehouse by ID
/// </summary>
public class GetWarehouseByIdQuery : IRequest<WarehouseDto?>
{
    /// <summary>
    /// Warehouse ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Include inventory items in the response
    /// </summary>
    public bool IncludeInventory { get; set; } = false;

    /// <summary>
    /// Include transaction history in the response
    /// </summary>
    public bool IncludeTransactions { get; set; } = false;
}

/// <summary>
/// Handler for GetWarehouseByIdQuery
/// </summary>
public class GetWarehouseByIdQueryHandler : IRequestHandler<GetWarehouseByIdQuery, WarehouseDto?>
{
    private readonly IApplicationDbContext _context;

    public GetWarehouseByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WarehouseDto?> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Warehouses.AsQueryable();

        // Include related data based on request parameters
        if (request.IncludeInventory)
        {
            query = query.Include(w => w.InventoryItems)
                         .ThenInclude(i => i.Product);
        }

        if (request.IncludeTransactions)
        {
            query = query.Include(w => w.Transactions)
                         .ThenInclude(t => t.Product);
        }

        var warehouse = await query
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
                UpdatedAt = w.UpdatedAt,
                InventoryItemCount = w.InventoryItems.Count,
                CapacityUtilization = w.Capacity.HasValue && w.Capacity.Value > 0 
                    ? (decimal)w.InventoryItems.Sum(i => i.Quantity) / w.Capacity.Value * 100 
                    : null
            })
            .FirstOrDefaultAsync(cancellationToken);

        return warehouse;
    }
}
