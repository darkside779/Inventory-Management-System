using MediatR;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Enums;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.Inventory.Commands.AdjustStock;

/// <summary>
/// Command to adjust inventory stock levels
/// </summary>
public class AdjustStockCommand : IRequest<AdjustStockCommandResponse>
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
    /// Quantity to adjust (positive for increase, negative for decrease)
    /// </summary>
    public int AdjustmentQuantity { get; set; }
    
    /// <summary>
    /// Reason for the adjustment
    /// </summary>
    public string Reason { get; set; } = string.Empty;
    
    /// <summary>
    /// Reference number for the adjustment
    /// </summary>
    public string? ReferenceNumber { get; set; }
    
    /// <summary>
    /// User performing the adjustment
    /// </summary>
    public int UserId { get; set; }
}

/// <summary>
/// Response for AdjustStockCommand
/// </summary>
public class AdjustStockCommandResponse
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
    /// New inventory quantity after adjustment
    /// </summary>
    public int NewQuantity { get; set; }
    
    /// <summary>
    /// Transaction ID created for this adjustment
    /// </summary>
    public int TransactionId { get; set; }
}

/// <summary>
/// Handler for AdjustStockCommand
/// </summary>
public class AdjustStockCommandHandler : IRequestHandler<AdjustStockCommand, AdjustStockCommandResponse>
{
    private readonly IApplicationDbContext _context;

    public AdjustStockCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdjustStockCommandResponse> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find or create inventory record
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == request.ProductId && i.WarehouseId == request.WarehouseId, cancellationToken);

            if (inventory == null)
            {
                // Create new inventory record if it doesn't exist
                inventory = new Domain.Entities.Inventory
                {
                    ProductId = request.ProductId,
                    WarehouseId = request.WarehouseId,
                    Quantity = 0,
                    ReservedQuantity = 0,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Inventories.Add(inventory);
            }

            // Validate the adjustment
            if (!inventory.AdjustQuantity(request.AdjustmentQuantity))
            {
                return new AdjustStockCommandResponse
                {
                    Success = false,
                    ErrorMessage = "Invalid stock adjustment. This would result in negative inventory or insufficient available stock."
                };
            }

            // Create transaction record
            var transaction = Transaction.CreateAdjustment(
                request.ProductId,
                request.WarehouseId,
                request.UserId,
                request.AdjustmentQuantity,
                inventory.Quantity,
                request.Reason
            );

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync(cancellationToken);

            return new AdjustStockCommandResponse
            {
                Success = true,
                NewQuantity = inventory.Quantity,
                TransactionId = transaction.Id
            };
        }
        catch (Exception ex)
        {
            return new AdjustStockCommandResponse
            {
                Success = false,
                ErrorMessage = $"An error occurred while adjusting stock: {ex.Message}"
            };
        }
    }
}
