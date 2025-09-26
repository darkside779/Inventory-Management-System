using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Customers.Commands;

/// <summary>
/// Command to toggle customer active status
/// </summary>
public class ToggleCustomerStatusCommand : IRequest<ToggleCustomerStatusCommandResponse>
{
    /// <summary>
    /// Customer ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User who updated the customer
    /// </summary>
    public int UpdatedByUserId { get; set; }
}

/// <summary>
/// Response for ToggleCustomerStatusCommand
/// </summary>
public class ToggleCustomerStatusCommandResponse
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
/// Handler for ToggleCustomerStatusCommand
/// </summary>
public class ToggleCustomerStatusCommandHandler : IRequestHandler<ToggleCustomerStatusCommand, ToggleCustomerStatusCommandResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ToggleCustomerStatusCommandHandler> _logger;

    public ToggleCustomerStatusCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ToggleCustomerStatusCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ToggleCustomerStatusCommandResponse> Handle(ToggleCustomerStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Toggling customer status: {CustomerId}", request.Id);

            // Get existing customer
            var customer = await _unitOfWork.Customers.GetByIdAsync(request.Id, cancellationToken: cancellationToken);
            if (customer == null)
            {
                return new ToggleCustomerStatusCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Customer not found"
                };
            }

            // Toggle active status
            customer.IsActive = !customer.IsActive;

            // Save changes
            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Customer status toggled successfully: {CustomerCode} - Active: {IsActive}", 
                customer.CustomerCode, customer.IsActive);

            return new ToggleCustomerStatusCommandResponse
            {
                IsSuccess = true,
                Customer = _mapper.Map<CustomerDto>(customer)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling customer status: {CustomerId}", request.Id);
            return new ToggleCustomerStatusCommandResponse
            {
                IsSuccess = false,
                ErrorMessage = "An error occurred while updating customer status"
            };
        }
    }
}
