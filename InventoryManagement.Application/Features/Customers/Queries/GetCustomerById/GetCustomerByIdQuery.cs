using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Customers.Queries;

/// <summary>
/// Query to get customer by ID
/// </summary>
public class GetCustomerByIdQuery : IRequest<GetCustomerByIdQueryResponse>
{
    /// <summary>
    /// Customer ID
    /// </summary>
    public int Id { get; set; }
}

/// <summary>
/// Response for GetCustomerByIdQuery
/// </summary>
public class GetCustomerByIdQueryResponse
{
    /// <summary>
    /// Customer data
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
/// Handler for GetCustomerByIdQuery
/// </summary>
public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, GetCustomerByIdQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCustomerByIdQueryHandler> _logger;

    public GetCustomerByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetCustomerByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetCustomerByIdQueryResponse> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting customer by ID: {CustomerId}", request.Id);

            var customer = await _unitOfWork.Customers.GetByIdAsync(request.Id, cancellationToken: cancellationToken);

            if (customer == null)
            {
                _logger.LogWarning("Customer not found: {CustomerId}", request.Id);
                return new GetCustomerByIdQueryResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Customer not found"
                };
            }

            var customerDto = _mapper.Map<CustomerDto>(customer);

            _logger.LogInformation("Successfully retrieved customer: {CustomerCode}", customer.CustomerCode);

            return new GetCustomerByIdQueryResponse
            {
                IsSuccess = true,
                Customer = customerDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer by ID: {CustomerId}", request.Id);
            return new GetCustomerByIdQueryResponse
            {
                IsSuccess = false,
                ErrorMessage = "An error occurred while retrieving the customer"
            };
        }
    }
}
