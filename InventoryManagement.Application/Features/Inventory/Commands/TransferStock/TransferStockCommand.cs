using MediatR;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Enums;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.Inventory.Commands.TransferStock;

/// <summary>
/// Command to transfer stock between warehouses
/// </summary>
public class TransferStockCommand : IRequest<TransferStockCommandResponse>
{
    /// <summary>
    /// Product ID to transfer
    /// </summary>
    public int ProductId { get; set; }
    
    /// <summary>
    /// Source warehouse ID
    /// </summary>
    public int SourceWarehouseId { get; set; }
    
    /// <summary>
    /// Destination warehouse ID
    /// </summary>
    public int DestinationWarehouseId { get; set; }
    
    /// <summary>
    /// Quantity to transfer
    /// </summary>
    public int Quantity { get; set; }
    
    /// <summary>
    /// Reason for the transfer
    /// </summary>
    public string Reason { get; set; } = string.Empty;
    
    /// <summary>
    /// Reference number for the transfer
    /// </summary>
    public string? ReferenceNumber { get; set; }
    
    /// <summary>
    /// User performing the transfer
    /// </summary>
    public int UserId { get; set; }
}

/// <summary>
/// Response for TransferStockCommand
/// </summary>
public class TransferStockCommandResponse
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
    /// Source warehouse remaining quantity
    /// </summary>
    public int SourceQuantity { get; set; }
    
    /// <summary>
    /// Destination warehouse new quantity
    /// </summary>
    public int DestinationQuantity { get; set; }
    
    /// <summary>
    /// Out transaction ID from source warehouse
    /// </summary>
    public int OutTransactionId { get; set; }
    
    /// <summary>
    /// In transaction ID to destination warehouse
    /// </summary>
    public int InTransactionId { get; set; }
}

/// <summary>
/// Handler for TransferStockCommand
/// </summary>
public class TransferStockCommandHandler : IRequestHandler<TransferStockCommand, TransferStockCommandResponse>
{
    private readonly IApplicationDbContext _context;

    public TransferStockCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TransferStockCommandResponse> Handle(TransferStockCommand request, CancellationToken cancellationToken)
    {
        // Note: Using DbContext directly for transaction management
        using var dbTransaction = await ((Microsoft.EntityFrameworkCore.DbContext)_context).Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            // Validate source and destination are different
            if (request.SourceWarehouseId == request.DestinationWarehouseId)
            {
                return new TransferStockCommandResponse
                {
                    Success = false,
                    ErrorMessage = "Source and destination warehouses cannot be the same."
                };
            }

            // Find source inventory
            var sourceInventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == request.ProductId && i.WarehouseId == request.SourceWarehouseId, cancellationToken);

            if (sourceInventory == null)
            {
                return new TransferStockCommandResponse
                {
                    Success = false,
                    ErrorMessage = "Source inventory record not found."
                };
            }

            // Validate sufficient stock in source
            if (!sourceInventory.HasSufficientStock(request.Quantity))
            {
                return new TransferStockCommandResponse
                {
                    Success = false,
                    ErrorMessage = $"Insufficient stock in source warehouse. Available: {sourceInventory.AvailableQuantity}, Requested: {request.Quantity}"
                };
            }

            // Find or create destination inventory
            var destinationInventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == request.ProductId && i.WarehouseId == request.DestinationWarehouseId, cancellationToken);

            if (destinationInventory == null)
            {
                destinationInventory = new Domain.Entities.Inventory
                {
                    ProductId = request.ProductId,
                    WarehouseId = request.DestinationWarehouseId,
                    Quantity = 0,
                    ReservedQuantity = 0,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Inventories.Add(destinationInventory);
            }

            // Perform the transfer (adjust quantities)
            if (!sourceInventory.AdjustQuantity(-request.Quantity) || 
                !destinationInventory.AdjustQuantity(request.Quantity))
            {
                return new TransferStockCommandResponse
                {
                    Success = false,
                    ErrorMessage = "Failed to adjust inventory quantities during transfer."
                };
            }

            // Create source transaction (stock out)
            var sourceTransaction = Transaction.CreateStockOut(
                request.ProductId,
                request.SourceWarehouseId,
                request.UserId,
                request.Quantity,
                sourceInventory.Quantity,
                null,
                $"Stock transfer to warehouse {destinationInventory.Warehouse.Name}",
                request.ReferenceNumber
            );

            // Create destination transaction (stock in)
            var destinationTransaction = Transaction.CreateStockIn(
                request.ProductId,
                request.DestinationWarehouseId,
                request.UserId,
                request.Quantity,
                destinationInventory.Quantity,
                null,
                $"Stock transfer from warehouse {sourceInventory.Warehouse.Name}",
                request.ReferenceNumber
            );

            _context.Transactions.Add(sourceTransaction);
            _context.Transactions.Add(destinationTransaction);

            await _context.SaveChangesAsync(cancellationToken);
            await dbTransaction.CommitAsync(cancellationToken);

            return new TransferStockCommandResponse
            {
                Success = true,
                SourceQuantity = sourceInventory.Quantity,
                DestinationQuantity = destinationInventory.Quantity,
                OutTransactionId = sourceTransaction.Id,
                InTransactionId = destinationTransaction.Id
            };
        }
        catch (Exception ex)
        {
            await dbTransaction.RollbackAsync(cancellationToken);
            return new TransferStockCommandResponse
            {
                Success = false,
                ErrorMessage = $"An error occurred during stock transfer: {ex.Message}"
            };
        }
    }
}
