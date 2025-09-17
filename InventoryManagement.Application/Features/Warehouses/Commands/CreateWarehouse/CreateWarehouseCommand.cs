using MediatR;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.Warehouses.Commands.CreateWarehouse;

/// <summary>
/// Command to create a new warehouse
/// </summary>
public class CreateWarehouseCommand : IRequest<CreateWarehouseCommandResponse>
{
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
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// User creating the warehouse
    /// </summary>
    public int UserId { get; set; }
}

/// <summary>
/// Response for CreateWarehouseCommand
/// </summary>
public class CreateWarehouseCommandResponse
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
    /// ID of the created warehouse
    /// </summary>
    public int WarehouseId { get; set; }

    /// <summary>
    /// Name of the created warehouse
    /// </summary>
    public string? WarehouseName { get; set; }
}

/// <summary>
/// Handler for CreateWarehouseCommand
/// </summary>
public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, CreateWarehouseCommandResponse>
{
    private readonly IApplicationDbContext _context;

    public CreateWarehouseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CreateWarehouseCommandResponse> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if warehouse name already exists
            var existingWarehouse = await _context.Warehouses
                .Where(w => w.Name.ToLower() == request.Name.ToLower())
                .FirstOrDefaultAsync(cancellationToken);

            if (existingWarehouse != null)
            {
                return new CreateWarehouseCommandResponse
                {
                    Success = false,
                    ErrorMessage = "A warehouse with this name already exists."
                };
            }

            // Validate capacity if provided
            if (request.Capacity.HasValue && request.Capacity.Value <= 0)
            {
                return new CreateWarehouseCommandResponse
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
                    return new CreateWarehouseCommandResponse
                    {
                        Success = false,
                        ErrorMessage = "Invalid email address format."
                    };
                }
            }

            // Create new warehouse entity
            var warehouse = new Warehouse
            {
                Name = request.Name.Trim(),
                Location = request.Location.Trim(),
                Address = request.Address?.Trim(),
                ContactPhone = request.ContactPhone?.Trim(),
                ContactEmail = request.ContactEmail?.Trim(),
                Capacity = request.Capacity,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreateWarehouseCommandResponse
            {
                Success = true,
                WarehouseId = warehouse.Id,
                WarehouseName = warehouse.Name
            };
        }
        catch (Exception ex)
        {
            return new CreateWarehouseCommandResponse
            {
                Success = false,
                ErrorMessage = $"An error occurred while creating the warehouse: {ex.Message}"
            };
        }
    }
}
