using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.Suppliers.Commands.DeleteSupplier;

/// <summary>
/// Command to delete (deactivate) a supplier
/// </summary>
public class DeleteSupplierCommand : IRequest<DeleteSupplierCommandResponse>
{
    /// <summary>
    /// Supplier ID to delete
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User ID performing the deletion
    /// </summary>
    public int UserId { get; set; }

    public DeleteSupplierCommand(int id, int userId)
    {
        Id = id;
        UserId = userId;
    }
}

/// <summary>
/// Response for DeleteSupplierCommand
/// </summary>
public class DeleteSupplierCommandResponse
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Whether the supplier was found
    /// </summary>
    public bool IsFound { get; set; } = true;

    /// <summary>
    /// Validation errors
    /// </summary>
    public Dictionary<string, string[]> ValidationErrors { get; set; } = new();
}

/// <summary>
/// Handler for DeleteSupplierCommand
/// </summary>
public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, DeleteSupplierCommandResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<DeleteSupplierCommandHandler> _logger;

    public DeleteSupplierCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DeleteSupplierCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DeleteSupplierCommandResponse> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get supplier with products
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(request.Id, cancellationToken);

            if (supplier == null)
            {
                _logger.LogWarning("Supplier with ID {SupplierId} not found", request.Id);
                return new DeleteSupplierCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Supplier not found."
                };
            }

            // Check for active products
            if (supplier.Products.Any(p => p.IsActive))
            {
                _logger.LogWarning("Cannot delete supplier {SupplierId} with active products", request.Id);
                return new DeleteSupplierCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Cannot delete supplier with active products. Please deactivate or reassign products first."
                };
            }

            // Check for products with inventory
            if (supplier.Products.Any(p => p.InventoryItems.Any(i => i.Quantity > 0)))
            {
                _logger.LogWarning("Cannot delete supplier {SupplierId} with inventory stock", request.Id);
                return new DeleteSupplierCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Cannot delete supplier with products that have inventory stock."
                };
            }

            // Check for recent transactions (within last 30 days)
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            if (supplier.Products.Any(p => p.Transactions.Any(t => t.CreatedAt >= thirtyDaysAgo)))
            {
                _logger.LogWarning("Cannot delete supplier {SupplierId} with recent transactions", request.Id);
                return new DeleteSupplierCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Cannot delete supplier with products that have recent transactions."
                };
            }

            // Soft delete - deactivate the supplier
            supplier.IsActive = false;
            supplier.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Suppliers.Update(supplier);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Supplier soft deleted: {SupplierName} (ID: {SupplierId}) by User {UserId}", 
                supplier.Name, supplier.Id, request.UserId);

            return new DeleteSupplierCommandResponse
            {
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting supplier {SupplierId} by User {UserId}", request.Id, request.UserId);
            return new DeleteSupplierCommandResponse
            {
                IsSuccess = false,
                ErrorMessage = "An error occurred while deleting the supplier. Please try again."
            };
        }
    }
}
