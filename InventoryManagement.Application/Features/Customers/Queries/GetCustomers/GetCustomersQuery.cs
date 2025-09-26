using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Customers.Queries;

/// <summary>
/// Query to get customers with filtering and pagination
/// </summary>
public class GetCustomersQuery : IRequest<GetCustomersQueryResponse>
{
    /// <summary>
    /// Search term (name, code, email, phone)
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Customer type filter
    /// </summary>
    public string? CustomerType { get; set; }

    /// <summary>
    /// Active status filter
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Registration date from
    /// </summary>
    public DateTime? RegisteredFrom { get; set; }

    /// <summary>
    /// Registration date to
    /// </summary>
    public DateTime? RegisteredTo { get; set; }

    /// <summary>
    /// Page number
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Sort field
    /// </summary>
    public string SortBy { get; set; } = "FullName";

    /// <summary>
    /// Sort direction
    /// </summary>
    public string SortDirection { get; set; } = "asc";
}

/// <summary>
/// Response for GetCustomersQuery
/// </summary>
public class GetCustomersQueryResponse
{
    /// <summary>
    /// List of customers
    /// </summary>
    public IEnumerable<CustomerDto> Data { get; set; } = new List<CustomerDto>();

    /// <summary>
    /// Total count of customers
    /// </summary>
    public int TotalCount { get; set; }

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
/// Handler for GetCustomersQuery
/// </summary>
public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, GetCustomersQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCustomersQueryHandler> _logger;

    public GetCustomersQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetCustomersQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetCustomersQueryResponse> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting customers with filters - SearchTerm: {SearchTerm}, Type: {Type}, IsActive: {IsActive}",
                request.SearchTerm, request.CustomerType, request.IsActive);

            var (customers, totalCount) = await _unitOfWork.Customers.SearchCustomersAsync(
                searchTerm: request.SearchTerm,
                customerType: request.CustomerType,
                isActive: request.IsActive,
                registeredFrom: request.RegisteredFrom,
                registeredTo: request.RegisteredTo,
                page: request.Page,
                pageSize: request.PageSize,
                sortBy: request.SortBy,
                sortDirection: request.SortDirection,
                cancellationToken: cancellationToken);

            var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);

            _logger.LogInformation("Retrieved {Count} customers out of {Total} total", customerDtos.Count(), totalCount);

            return new GetCustomersQueryResponse
            {
                IsSuccess = true,
                Data = customerDtos,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers");
            return new GetCustomersQueryResponse
            {
                IsSuccess = false,
                ErrorMessage = "An error occurred while retrieving customers"
            };
        }
    }
}
