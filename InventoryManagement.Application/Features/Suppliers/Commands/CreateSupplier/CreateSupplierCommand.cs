using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.Suppliers.Commands.CreateSupplier;

/// <summary>
/// Command to create a new supplier
/// </summary>
public class CreateSupplierCommand : IRequest<CreateSupplierCommandResponse>
{
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
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// User ID creating the supplier
    /// </summary>
    public int UserId { get; set; }
}

/// <summary>
/// Response for CreateSupplierCommand
/// </summary>
public class CreateSupplierCommandResponse
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
    /// Created supplier ID
    /// </summary>
    public int SupplierId { get; set; }

    /// <summary>
    /// Validation errors
    /// </summary>
    public Dictionary<string, string[]> ValidationErrors { get; set; } = new();
}

/// <summary>
/// Handler for CreateSupplierCommand
/// </summary>
public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, CreateSupplierCommandResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSupplierCommandHandler> _logger;

    public CreateSupplierCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateSupplierCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateSupplierCommandResponse> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return new CreateSupplierCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Supplier name is required",
                    ValidationErrors = new Dictionary<string, string[]>
                    {
                        { "Name", new[] { "Supplier name is required" } }
                    }
                };
            }

            // Check for duplicate name
            var existingSupplier = await _unitOfWork.Suppliers.GetFirstOrDefaultAsync(
                s => s.Name.ToLower() == request.Name.ToLower(),
                cancellationToken: cancellationToken);

            if (existingSupplier != null)
            {
                return new CreateSupplierCommandResponse
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
                return new CreateSupplierCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid email format",
                    ValidationErrors = new Dictionary<string, string[]>
                    {
                        { "Email", new[] { "Invalid email format" } }
                    }
                };
            }

            // Check for duplicate email if provided
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var existingEmail = await _unitOfWork.Suppliers.GetFirstOrDefaultAsync(
                    s => s.Email != null && s.Email.ToLower() == request.Email.ToLower(),
                    cancellationToken: cancellationToken);

                if (existingEmail != null)
                {
                    return new CreateSupplierCommandResponse
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

            // Validate website URL format if provided
            if (!string.IsNullOrWhiteSpace(request.Website) && !IsValidUrl(request.Website))
            {
                return new CreateSupplierCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid website URL format",
                    ValidationErrors = new Dictionary<string, string[]>
                    {
                        { "Website", new[] { "Invalid website URL format" } }
                    }
                };
            }

            // Create the supplier entity
            var supplier = new Supplier
            {
                Name = request.Name.Trim(),
                ContactInfo = string.IsNullOrWhiteSpace(request.ContactInfo) ? null : request.ContactInfo.Trim(),
                Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim().ToLower(),
                Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
                Website = string.IsNullOrWhiteSpace(request.Website) ? null : request.Website.Trim(),
                TaxNumber = string.IsNullOrWhiteSpace(request.TaxNumber) ? null : request.TaxNumber.Trim(),
                PaymentTerms = string.IsNullOrWhiteSpace(request.PaymentTerms) ? null : request.PaymentTerms.Trim(),
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add to database
            await _unitOfWork.Suppliers.AddAsync(supplier);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var supplierDto = _mapper.Map<SupplierDto>(supplier);

            _logger.LogInformation("Created supplier: {SupplierName} with ID: {SupplierId}", 
                supplier.Name, supplier.Id);

            return new CreateSupplierCommandResponse
            {
                IsSuccess = true,
                SupplierId = supplier.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating supplier: {SupplierName} by User {UserId}", request.Name, request.UserId);
            return new CreateSupplierCommandResponse
            {
                IsSuccess = false,
                ErrorMessage = "An error occurred while creating the supplier. Please try again."
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
