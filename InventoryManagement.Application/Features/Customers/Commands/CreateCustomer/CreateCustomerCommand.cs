using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Customers.Commands;

/// <summary>
/// Command to create a new customer
/// </summary>
public class CreateCustomerCommand : IRequest<CreateCustomerCommandResponse>
{
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
    /// User who created the customer
    /// </summary>
    public int CreatedByUserId { get; set; }
}

/// <summary>
/// Response for CreateCustomerCommand
/// </summary>
public class CreateCustomerCommandResponse
{
    /// <summary>
    /// Created customer
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
/// Handler for CreateCustomerCommand
/// </summary>
public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CreateCustomerCommandResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateCustomerCommandHandler> _logger;

    public CreateCustomerCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateCustomerCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateCustomerCommandResponse> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating customer: {CustomerName}", request.FullName);

            // Validate input
            if (string.IsNullOrWhiteSpace(request.FullName))
            {
                return new CreateCustomerCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Customer name is required"
                };
            }

            // Generate customer code (simplified)
            var customerCode = Customer.GenerateCustomerCode(1);

            // Create customer entity
            var customer = new Customer
            {
                CustomerCode = customerCode,
                FullName = request.FullName,
                CompanyName = request.CompanyName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                CustomerType = request.CustomerType,
                CreditLimit = request.CreditLimit,
                PaymentTermsDays = request.PaymentTermsDays,
                TaxId = request.TaxId,
                Notes = request.Notes,
                IsActive = true,
                RegisteredDate = DateTime.UtcNow,
                Balance = 0,
                TotalPurchases = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Validate customer
            if (!customer.IsValid())
            {
                return new CreateCustomerCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Customer data validation failed"
                };
            }

            // Add to database
            await _unitOfWork.Customers.AddAsync(customer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Customer created successfully: {CustomerCode}", customerCode);

            return new CreateCustomerCommandResponse
            {
                IsSuccess = true,
                Customer = _mapper.Map<CustomerDto>(customer)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer: {CustomerName}", request.FullName);
            return new CreateCustomerCommandResponse
            {
                IsSuccess = false,
                ErrorMessage = "An error occurred while creating the customer"
            };
        }
    }
}
