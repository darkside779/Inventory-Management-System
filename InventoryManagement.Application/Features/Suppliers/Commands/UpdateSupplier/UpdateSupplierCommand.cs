using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.Suppliers.Commands.UpdateSupplier;

/// <summary>
/// Command to update an existing supplier
/// </summary>
public class UpdateSupplierCommand : IRequest<UpdateSupplierCommandResponse>
{
    /// <summary>
    /// Supplier ID to update
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the supplier
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Contact information
    /// </summary>
    public string? ContactInfo { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Email address
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Website URL
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// Tax identification number
    /// </summary>
    public string? TaxNumber { get; set; }

    /// <summary>
    /// Payment terms
    /// </summary>
    public string? PaymentTerms { get; set; }

    /// <summary>
    /// Is supplier active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// User ID updating the supplier
    /// </summary>
    public int UserId { get; set; }
}

/// <summary>
/// Response for UpdateSupplierCommand
/// </summary>
public class UpdateSupplierCommandResponse
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
    /// Validation errors
    /// </summary>
    public Dictionary<string, string[]> ValidationErrors { get; set; } = new();

    /// <summary>
    /// Whether the supplier was found
    /// </summary>
    public bool IsFound { get; set; } = true;
}

/// <summary>
/// Handler for UpdateSupplierCommand
/// </summary>
public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, UpdateSupplierCommandResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSupplierCommandHandler> _logger;

    public UpdateSupplierCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateSupplierCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UpdateSupplierCommandResponse> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return new UpdateSupplierCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Supplier name is required",
                    ValidationErrors = new Dictionary<string, string[]>
                    {
                        { "Name", new[] { "Supplier name is required" } }
                    }
                };
            }

            // Get existing supplier
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(request.Id, cancellationToken);

            if (supplier == null)
            {
                return new UpdateSupplierCommandResponse
                {
                    IsSuccess = false,
                    IsFound = false,
                    ErrorMessage = "Supplier not found"
                };
            }

            // Check for duplicate name (excluding current supplier)
            var existingSupplier = await _unitOfWork.Suppliers.GetFirstOrDefaultAsync(
                s => s.Name.ToLower() == request.Name.ToLower() && s.Id != request.Id,
                cancellationToken: cancellationToken);

            if (existingSupplier != null)
            {
                return new UpdateSupplierCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "A supplier with this name already exists",
                    ValidationErrors = new Dictionary<string, string[]>
                    {
                        { "Name", new[] { "A supplier with this name already exists" } }
                    }
                };
            }

            // Validate email format if provided
            if (!string.IsNullOrWhiteSpace(request.Email) && !IsValidEmail(request.Email))
            {
                return new UpdateSupplierCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid email format",
                    ValidationErrors = new Dictionary<string, string[]>
                    {
                        { "Email", new[] { "Invalid email format" } }
                    }
                };
            }

            // Check for duplicate email if provided (excluding current supplier)
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var existingEmailSupplier = await _unitOfWork.Suppliers.GetFirstOrDefaultAsync(
                    s => s.Id != request.Id && s.Email != null && s.Email.ToLower() == request.Email.ToLower(),
                    cancellationToken: cancellationToken);

                if (existingEmailSupplier != null)
                {
                    return new UpdateSupplierCommandResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "A supplier with this email already exists",
                        ValidationErrors = new Dictionary<string, string[]>
                        {
                            { "Email", new[] { "A supplier with this email already exists" } }
                        }
                    };
                }
            }

            // Validate website URL if provided
            if (!string.IsNullOrWhiteSpace(request.Website) && !IsValidUrl(request.Website))
            {
                return new UpdateSupplierCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid website URL format",
                    ValidationErrors = new Dictionary<string, string[]>
                    {
                        { "Website", new[] { "Invalid website URL format" } }
                    }
                };
            }

            // Check if trying to deactivate supplier with active products
            if (!request.IsActive && supplier.IsActive && supplier.Products.Any(p => p.IsActive))
            {
                return new UpdateSupplierCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Cannot deactivate supplier with active products. Please deactivate or reassign all products first.",
                    ValidationErrors = new Dictionary<string, string[]>
                    {
                        { "IsActive", new[] { "Cannot deactivate supplier with active products" } }
                    }
                };
            }

            // Update supplier properties
            supplier.Name = request.Name.Trim();
            supplier.ContactInfo = string.IsNullOrWhiteSpace(request.ContactInfo) ? null : request.ContactInfo.Trim();
            supplier.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
            supplier.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim().ToLower();
            supplier.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();
            supplier.Website = string.IsNullOrWhiteSpace(request.Website) ? null : request.Website.Trim();
            supplier.TaxNumber = string.IsNullOrWhiteSpace(request.TaxNumber) ? null : request.TaxNumber.Trim();
            supplier.PaymentTerms = string.IsNullOrWhiteSpace(request.PaymentTerms) ? null : request.PaymentTerms.Trim();
            supplier.IsActive = request.IsActive;
            supplier.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Suppliers.Update(supplier);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Supplier updated successfully: {SupplierName} (ID: {SupplierId}) by User {UserId}", 
                supplier.Name, supplier.Id, request.UserId);

            return new UpdateSupplierCommandResponse
            {
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating supplier {SupplierId} by User {UserId}", request.Id, request.UserId);
            return new UpdateSupplierCommandResponse
            {
                IsSuccess = false,
                ErrorMessage = "An error occurred while updating the supplier. Please try again."
            };
        }
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
