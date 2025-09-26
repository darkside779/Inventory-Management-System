using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.CustomerInvoices.Queries;

/// <summary>
/// Query to get invoices with filtering and pagination
/// </summary>
public class GetInvoicesQuery : IRequest<GetInvoicesQueryResponse>
{
    /// <summary>
    /// Search term
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Customer ID filter
    /// </summary>
    public int? CustomerId { get; set; }

    /// <summary>
    /// Invoice status filter
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Invoice date from
    /// </summary>
    public DateTime? InvoiceDateFrom { get; set; }

    /// <summary>
    /// Invoice date to
    /// </summary>
    public DateTime? InvoiceDateTo { get; set; }

    /// <summary>
    /// Due date from
    /// </summary>
    public DateTime? DueDateFrom { get; set; }

    /// <summary>
    /// Due date to
    /// </summary>
    public DateTime? DueDateTo { get; set; }

    /// <summary>
    /// Only overdue invoices
    /// </summary>
    public bool? IsOverdue { get; set; }

    /// <summary>
    /// Page number
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Sort by field
    /// </summary>
    public string SortBy { get; set; } = "InvoiceDate";

    /// <summary>
    /// Sort direction
    /// </summary>
    public string SortDirection { get; set; } = "desc";
}

/// <summary>
/// Response for GetInvoicesQuery
/// </summary>
public class GetInvoicesQueryResponse
{
    /// <summary>
    /// Invoice data
    /// </summary>
    public IEnumerable<CustomerInvoiceDto> Data { get; set; } = new List<CustomerInvoiceDto>();

    /// <summary>
    /// Total count
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
/// Handler for GetInvoicesQuery
/// </summary>
public class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, GetInvoicesQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetInvoicesQueryHandler> _logger;

    public GetInvoicesQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetInvoicesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetInvoicesQueryResponse> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting invoices with filters - SearchTerm: {SearchTerm}, Status: {Status}, CustomerId: {CustomerId}",
                request.SearchTerm, request.Status, request.CustomerId);

            // Get invoices with filtering
            var (invoices, totalCount) = await _unitOfWork.CustomerInvoices.SearchInvoicesAsync(
                searchTerm: request.SearchTerm,
                customerId: request.CustomerId,
                status: request.Status,
                invoiceDateFrom: request.InvoiceDateFrom,
                invoiceDateTo: request.InvoiceDateTo,
                dueDateFrom: request.DueDateFrom,
                dueDateTo: request.DueDateTo,
                isOverdue: request.IsOverdue,
                page: request.Page,
                pageSize: request.PageSize,
                sortBy: request.SortBy,
                sortDirection: request.SortDirection,
                cancellationToken: cancellationToken);

            var invoiceDtos = _mapper.Map<IEnumerable<CustomerInvoiceDto>>(invoices);

            _logger.LogInformation("Retrieved {Count} invoices out of {Total} total", invoiceDtos.Count(), totalCount);

            return new GetInvoicesQueryResponse
            {
                IsSuccess = true,
                Data = invoiceDtos,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving invoices");
            return new GetInvoicesQueryResponse
            {
                IsSuccess = false,
                ErrorMessage = "An error occurred while retrieving invoices"
            };
        }
    }
}
