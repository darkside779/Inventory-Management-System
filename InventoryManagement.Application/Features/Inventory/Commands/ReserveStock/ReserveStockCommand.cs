using MediatR;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.Application.Features.Inventory.Commands.ReserveStock;

/// <summary>
/// Command to reserve stock for pending orders
/// </summary>
public class ReserveStockCommand : IRequest<ReserveStockCommandResponse>
{
    /// <summary>
    /// Product ID
    /// </summary>
    public int ProductId { get; set; }
    
    /// <summary>
    /// Warehouse ID
    /// </summary>
    public int WarehouseId { get; set; }
    
    /// <summary>
    /// Quantity to reserve
    /// </summary>
    public int Quantity { get; set; }
    
    /// <summary>
    /// Reason for the reservation
    /// </summary>
    public string Reason { get; set; } = string.Empty;
    
    /// <summary>
    /// Reference number for the reservation (e.g., Order ID)
    /// </summary>
    public string? ReferenceNumber { get; set; }
    
    /// <summary>
    /// User performing the reservation
    /// </summary>
    public int UserId { get; set; }
}

/// <summary>
/// Response for ReserveStockCommand
/// </summary>
public class ReserveStockCommandResponse
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// New reserved quantity after reservation
    /// </summary>
    public int NewReservedQuantity { get; set; }
    
    /// <summary>
    /// Available quantity after reservation
    /// </summary>
    public int AvailableQuantity { get; set; }
}

/// <summary>
/// Handler for ReserveStockCommand
/// </summary>
public class ReserveStockCommandHandler : IRequestHandler<ReserveStockCommand, ReserveStockCommandResponse>
{
    private readonly IApplicationDbContext _context;

    public ReserveStockCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReserveStockCommandResponse> Handle(ReserveStockCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find inventory record
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == request.ProductId && i.WarehouseId == request.WarehouseId, cancellationToken);

            if (inventory == null)
            {
                return new ReserveStockCommandResponse
                {
                    Success = false,
                    ErrorMessage = "Inventory record not found for the specified product and warehouse."
                };
            }

            // Validate and reserve quantity
            if (!inventory.ReserveQuantity(request.Quantity))
            {
                return new ReserveStockCommandResponse
                {
                    Success = false,
                    ErrorMessage = $"Insufficient stock available. Available quantity: {inventory.AvailableQuantity}, Requested: {request.Quantity}"
                };
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new ReserveStockCommandResponse
            {
                Success = true,
                NewReservedQuantity = inventory.ReservedQuantity,
                AvailableQuantity = inventory.AvailableQuantity
            };
        }
        catch (Exception ex)
        {
            return new ReserveStockCommandResponse
            {
                Success = false,
                ErrorMessage = $"An error occurred while reserving stock: {ex.Message}"
            };
        }
    }
}

/// <summary>
/// Command to release reserved stock
/// </summary>
public class ReleaseReservedStockCommand : IRequest<ReleaseReservedStockCommandResponse>
{
    /// <summary>
    /// Product ID
    /// </summary>
    public int ProductId { get; set; }
    
    /// <summary>
    /// Warehouse ID
    /// </summary>
    public int WarehouseId { get; set; }
    
    /// <summary>
    /// Quantity to release from reservation
    /// </summary>
    public int Quantity { get; set; }
    
    /// <summary>
    /// Reason for releasing the reservation
    /// </summary>
    public string Reason { get; set; } = string.Empty;
    
    /// <summary>
    /// Reference number for the release
    /// </summary>
    public string? ReferenceNumber { get; set; }
    
    /// <summary>
    /// User performing the release
    /// </summary>
    public string UserId { get; set; } = string.Empty;
}

/// <summary>
/// Response for ReleaseReservedStockCommand
/// </summary>
public class ReleaseReservedStockCommandResponse
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// New reserved quantity after release
    /// </summary>
    public int NewReservedQuantity { get; set; }
    
    /// <summary>
    /// Available quantity after release
    /// </summary>
    public int AvailableQuantity { get; set; }
}

/// <summary>
/// Handler for ReleaseReservedStockCommand
/// </summary>
public class ReleaseReservedStockCommandHandler : IRequestHandler<ReleaseReservedStockCommand, ReleaseReservedStockCommandResponse>
{
    private readonly IApplicationDbContext _context;

    public ReleaseReservedStockCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReleaseReservedStockCommandResponse> Handle(ReleaseReservedStockCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find inventory record
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == request.ProductId && i.WarehouseId == request.WarehouseId, cancellationToken);

            if (inventory == null)
            {
                return new ReleaseReservedStockCommandResponse
                {
                    Success = false,
                    ErrorMessage = "Inventory record not found for the specified product and warehouse."
                };
            }

            // Validate and release reserved quantity
            if (!inventory.ReleaseReservedQuantity(request.Quantity))
            {
                return new ReleaseReservedStockCommandResponse
                {
                    Success = false,
                    ErrorMessage = $"Cannot release more than reserved quantity. Reserved quantity: {inventory.ReservedQuantity}, Requested release: {request.Quantity}"
                };
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new ReleaseReservedStockCommandResponse
            {
                Success = true,
                NewReservedQuantity = inventory.ReservedQuantity,
                AvailableQuantity = inventory.AvailableQuantity
            };
        }
        catch (Exception ex)
        {
            return new ReleaseReservedStockCommandResponse
            {
                Success = false,
                ErrorMessage = $"An error occurred while releasing reserved stock: {ex.Message}"
            };
        }
    }
}
