using MediatR;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.Application.Features.Warehouses.Commands.DeleteWarehouse;

/// <summary>
/// Command to delete a warehouse (soft delete)
/// </summary>
public class DeleteWarehouseCommand : IRequest<DeleteWarehouseCommandResponse>
{
    /// <summary>
    /// Warehouse ID to delete
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User performing the deletion
    /// </summary>
    public int UserId { get; set; }
}

/// <summary>
/// Response for DeleteWarehouseCommand
/// </summary>
public class DeleteWarehouseCommandResponse
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
    /// ID of the deleted warehouse
    /// </summary>
    public int WarehouseId { get; set; }

    /// <summary>
    /// Name of the deleted warehouse
    /// </summary>
    public string? WarehouseName { get; set; }
}

/// <summary>
/// Handler for DeleteWarehouseCommand
/// </summary>
public class DeleteWarehouseCommandHandler : IRequestHandler<DeleteWarehouseCommand, DeleteWarehouseCommandResponse>
{
    private readonly IApplicationDbContext _context;

    public DeleteWarehouseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DeleteWarehouseCommandResponse> Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find the warehouse to delete
            var warehouse = await _context.Warehouses
                .Include(w => w.InventoryItems)
                .Include(w => w.Transactions)
                .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

            if (warehouse == null)
            {
                return new DeleteWarehouseCommandResponse
                {
                    Success = false,
                    ErrorMessage = "Warehouse not found."
                };
            }

            // Check if warehouse has any active inventory items
            var hasActiveInventory = warehouse.InventoryItems.Any(i => i.IsActive && i.Quantity > 0);
            if (hasActiveInventory)
            {
                return new DeleteWarehouseCommandResponse
                {
                    Success = false,
                    ErrorMessage = "Cannot delete warehouse that contains active inventory items. Please transfer all inventory to other warehouses first."
                };
            }

            // Check if warehouse has any recent transactions (within last 30 days)
            var recentTransactions = warehouse.Transactions.Any(t => t.CreatedAt > DateTime.UtcNow.AddDays(-30));
            if (recentTransactions)
            {
                return new DeleteWarehouseCommandResponse
                {
                    Success = false,
                    ErrorMessage = "Cannot delete warehouse with recent transaction history. Consider deactivating the warehouse instead."
                };
            }

            // Perform soft delete by deactivating the warehouse
            warehouse.IsActive = false;
            warehouse.UpdatedAt = DateTime.UtcNow;

            // Also deactivate any remaining inventory items (with zero quantity)
            var remainingInventoryItems = warehouse.InventoryItems.Where(i => i.IsActive).ToList();
            foreach (var item in remainingInventoryItems)
            {
                item.IsActive = false;
                item.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new DeleteWarehouseCommandResponse
            {
                Success = true,
                WarehouseId = warehouse.Id,
                WarehouseName = warehouse.Name
            };
        }
        catch (Exception ex)
        {
            return new DeleteWarehouseCommandResponse
            {
                Success = false,
                ErrorMessage = $"An error occurred while deleting the warehouse: {ex.Message}"
            };
        }
    }
}
