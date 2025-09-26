using InventoryManagement.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Customers.Commands;

/// <summary>
/// Command to delete a customer
/// </summary>
public class DeleteCustomerCommand : IRequest<DeleteCustomerCommandResponse>
{
    /// <summary>
    /// Customer ID to delete
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User who deleted the customer
    /// </summary>
    public int DeletedByUserId { get; set; }
}

/// <summary>
/// Response for DeleteCustomerCommand
/// </summary>
public class DeleteCustomerCommandResponse
{
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
/// Handler for DeleteCustomerCommand
/// </summary>
public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, DeleteCustomerCommandResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCustomerCommandHandler> _logger;

    public DeleteCustomerCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteCustomerCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DeleteCustomerCommandResponse> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting customer: {CustomerId}", request.Id);

            // Get existing customer
            var customer = await _unitOfWork.Customers.GetByIdAsync(request.Id, cancellationToken: cancellationToken);
            if (customer == null)
            {
                return new DeleteCustomerCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Customer not found"
                };
            }

            // Check if customer has any invoices
            var invoices = await _unitOfWork.CustomerInvoices.GetByCustomerIdAsync(request.Id, cancellationToken);
            if (invoices.Any())
            {
                return new DeleteCustomerCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Cannot delete customer with existing invoices. Deactivate the customer instead."
                };
            }

            // Check if customer has any payments
            var payments = await _unitOfWork.CustomerPayments.GetByCustomerIdAsync(request.Id, cancellationToken);
            if (payments.Any())
            {
                return new DeleteCustomerCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Cannot delete customer with existing payment records. Deactivate the customer instead."
                };
            }

            // Soft delete - just deactivate the customer
            customer.IsActive = false;
            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Customer deactivated successfully: {CustomerCode}", customer.CustomerCode);

            return new DeleteCustomerCommandResponse
            {
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer: {CustomerId}", request.Id);
            return new DeleteCustomerCommandResponse
            {
                IsSuccess = false,
                ErrorMessage = "An error occurred while deleting the customer"
            };
        }
    }
}
