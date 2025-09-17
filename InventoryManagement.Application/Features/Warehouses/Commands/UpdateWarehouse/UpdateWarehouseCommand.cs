using MediatR;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.Application.Features.Warehouses.Commands.UpdateWarehouse;

/// <summary>
/// Command to update an existing warehouse
/// </summary>
public class UpdateWarehouseCommand : IRequest<UpdateWarehouseCommandResponse>
{
    /// <summary>
    /// Warehouse ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the warehouse
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Location of the warehouse
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Full address of the warehouse
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Contact phone number
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    /// Contact email
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// Storage capacity
    /// </summary>
    public int? Capacity { get; set; }

    /// <summary>
    /// Whether the warehouse is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// User updating the warehouse
    /// </summary>
    public int UserId { get; set; }
}

/// <summary>
/// Response for UpdateWarehouseCommand
/// </summary>
public class UpdateWarehouseCommandResponse
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
    /// ID of the updated warehouse
    /// </summary>
    public int WarehouseId { get; set; }

    /// <summary>
    /// Name of the updated warehouse
    /// </summary>
    public string? WarehouseName { get; set; }
}

/// <summary>
/// Handler for UpdateWarehouseCommand
/// </summary>
public class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommand, UpdateWarehouseCommandResponse>
{
    private readonly IApplicationDbContext _context;

    public UpdateWarehouseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateWarehouseCommandResponse> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find the warehouse to update
            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

            if (warehouse == null)
            {
                return new UpdateWarehouseCommandResponse
                {
                    Success = false,
                    ErrorMessage = "Warehouse not found."
                };
            }

            // Check if another warehouse has the same name (excluding current warehouse)
            var existingWarehouse = await _context.Warehouses
                .Where(w => w.Name.ToLower() == request.Name.ToLower() && w.Id != request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingWarehouse != null)
            {
                return new UpdateWarehouseCommandResponse
                {
                    Success = false,
                    ErrorMessage = "Another warehouse with this name already exists."
                };
            }

            // Validate capacity if provided
            if (request.Capacity.HasValue && request.Capacity.Value <= 0)
            {
                return new UpdateWarehouseCommandResponse
                {
                    Success = false,
                    ErrorMessage = "Warehouse capacity must be greater than zero."
                };
            }

            // Validate email format if provided
            if (!string.IsNullOrWhiteSpace(request.ContactEmail))
            {
                var emailAttribute = new System.ComponentModel.DataAnnotations.EmailAddressAttribute();
                if (!emailAttribute.IsValid(request.ContactEmail))
                {
                    return new UpdateWarehouseCommandResponse
                    {
                        Success = false,
                        ErrorMessage = "Invalid email address format."
                    };
                }
            }

            // Check if deactivating warehouse with active inventory
            if (!request.IsActive && warehouse.IsActive)
            {
                var hasActiveInventory = await _context.Inventories
                    .AnyAsync(i => i.WarehouseId == request.Id && i.IsActive && i.Quantity > 0, cancellationToken);

                if (hasActiveInventory)
                {
                    return new UpdateWarehouseCommandResponse
                    {
                        Success = false,
                        ErrorMessage = "Cannot deactivate warehouse that contains active inventory items."
                    };
                }
            }

            // Update warehouse properties
            warehouse.Name = request.Name.Trim();
            warehouse.Location = request.Location.Trim();
            warehouse.Address = request.Address?.Trim();
            warehouse.ContactPhone = request.ContactPhone?.Trim();
            warehouse.ContactEmail = request.ContactEmail?.Trim();
            warehouse.Capacity = request.Capacity;
            warehouse.IsActive = request.IsActive;
            warehouse.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return new UpdateWarehouseCommandResponse
            {
                Success = true,
                WarehouseId = warehouse.Id,
                WarehouseName = warehouse.Name
            };
        }
        catch (Exception ex)
        {
            return new UpdateWarehouseCommandResponse
            {
                Success = false,
                ErrorMessage = $"An error occurred while updating the warehouse: {ex.Message}"
            };
        }
    }
}
