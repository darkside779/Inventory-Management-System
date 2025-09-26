using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Customers.Commands;

/// <summary>
/// Command to update an existing customer
/// </summary>
public class UpdateCustomerCommand : IRequest<UpdateCustomerCommandResponse>
{
    /// <summary>
    /// Customer ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Company name
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// Email address
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Customer type
    /// </summary>
    public string CustomerType { get; set; } = "Individual";

    /// <summary>
    /// Credit limit
    /// </summary>
    public decimal CreditLimit { get; set; } = 0;

    /// <summary>
    /// Payment terms in days
    /// </summary>
    public int PaymentTermsDays { get; set; } = 30;

    /// <summary>
    /// Tax ID
    /// </summary>
    public string? TaxId { get; set; }

    /// <summary>
    /// Notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// User who updated the customer
    /// </summary>
    public int UpdatedByUserId { get; set; }
}

/// <summary>
/// Response for UpdateCustomerCommand
/// </summary>
public class UpdateCustomerCommandResponse
{
    /// <summary>
    /// Updated customer
    /// </summary>
    public CustomerDto? Customer { get; set; }

    /// <summary>
    /// Success flag
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Handler for UpdateCustomerCommand
/// </summary>
public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, UpdateCustomerCommandResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateCustomerCommandHandler> _logger;

    public UpdateCustomerCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateCustomerCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UpdateCustomerCommandResponse> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating customer: {CustomerId}", request.Id);

            // Get existing customer
            var customer = await _unitOfWork.Customers.GetByIdAsync(request.Id, cancellationToken: cancellationToken);
            if (customer == null)
            {
                return new UpdateCustomerCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Customer not found"
                };
            }

            // Check for duplicate email if changed
            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != customer.Email)
            {
                var existingCustomers = await _unitOfWork.Customers.GetAsync(
                    filter: c => c.Email == request.Email && c.IsActive && c.Id != request.Id,
                    cancellationToken: cancellationToken);

                if (existingCustomers.Any())
                {
                    return new UpdateCustomerCommandResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "A customer with this email already exists"
                    };
                }
            }

            // Update customer properties
            customer.FullName = request.FullName;
            customer.CompanyName = request.CompanyName;
            customer.Email = request.Email;
            customer.PhoneNumber = request.PhoneNumber;
            customer.Address = request.Address;
            customer.CustomerType = request.CustomerType;
            customer.CreditLimit = request.CreditLimit;
            customer.PaymentTermsDays = request.PaymentTermsDays;
            customer.TaxId = request.TaxId;
            customer.Notes = request.Notes;
            customer.IsActive = request.IsActive;

            // Validate customer
            if (!customer.IsValid())
            {
                return new UpdateCustomerCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Customer data validation failed"
                };
            }

            // Save changes
            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Customer updated successfully: {CustomerCode}", customer.CustomerCode);

            return new UpdateCustomerCommandResponse
            {
                IsSuccess = true,
                Customer = _mapper.Map<CustomerDto>(customer)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer: {CustomerId}", request.Id);
            return new UpdateCustomerCommandResponse
            {
                IsSuccess = false,
                ErrorMessage = "An error occurred while updating the customer"
            };
        }
    }
}
