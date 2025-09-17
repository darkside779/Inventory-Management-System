using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Transactions.Queries.GetAllTransactions;

/// <summary>
/// Query to get all transactions with filtering and pagination
/// </summary>
public class GetAllTransactionsQuery : IRequest<GetAllTransactionsQueryResponse>
{
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Search term for filtering
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter by transaction type
    /// </summary>
    public Domain.Enums.TransactionType? TransactionType { get; set; }

    /// <summary>
    /// Filter by product ID
    /// </summary>
    public int? ProductId { get; set; }

    /// <summary>
    /// Filter by warehouse ID
    /// </summary>
    public int? WarehouseId { get; set; }

    /// <summary>
    /// Filter by user ID
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Start date for date range filtering
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date for date range filtering
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Sort field
    /// </summary>
    public string? SortBy { get; set; } = "Timestamp";

    /// <summary>
    /// Sort direction (asc/desc)
    /// </summary>
    public string? SortDirection { get; set; } = "desc";
}

/// <summary>
/// Response for GetAllTransactionsQuery
/// </summary>
public class GetAllTransactionsQueryResponse
{
    /// <summary>
    /// List of transactions
    /// </summary>
    public IEnumerable<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();

    /// <summary>
    /// Total count of transactions (before pagination)
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Indicates if there are more pages
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Indicates if there are previous pages
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indicates if the query was successful
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    /// Error message if query failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Handler for GetAllTransactionsQuery
/// </summary>
public class GetAllTransactionsQueryHandler : IRequestHandler<GetAllTransactionsQuery, GetAllTransactionsQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllTransactionsQueryHandler> _logger;

    public GetAllTransactionsQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllTransactionsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetAllTransactionsQueryResponse> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting all transactions with filters - Page: {PageNumber}, Size: {PageSize}", 
                request.PageNumber, request.PageSize);

            // Build filter expressions
            var transactions = await _unitOfWork.Transactions.GetAsync(
                filter: t => 
                    (!request.ProductId.HasValue || t.ProductId == request.ProductId.Value) &&
                    (!request.WarehouseId.HasValue || t.WarehouseId == request.WarehouseId.Value) &&
                    (!request.UserId.HasValue || t.UserId == request.UserId.Value) &&
                    (!request.TransactionType.HasValue || t.TransactionType == request.TransactionType.Value) &&
                    (!request.StartDate.HasValue || t.Timestamp >= request.StartDate.Value) &&
                    (!request.EndDate.HasValue || t.Timestamp <= request.EndDate.Value) &&
                    (string.IsNullOrEmpty(request.SearchTerm) || 
                     (t.Product != null && t.Product.Name.Contains(request.SearchTerm)) ||
                     (t.Warehouse != null && t.Warehouse.Name.Contains(request.SearchTerm)) ||
                     (t.ReferenceNumber != null && t.ReferenceNumber.Contains(request.SearchTerm)) ||
                     (t.Notes != null && t.Notes.Contains(request.SearchTerm))),
                includeProperties: "Product,Warehouse,User",
                cancellationToken: cancellationToken);

            // Convert to list for further processing
            var transactionsList = transactions.ToList();

            // Apply sorting
            transactionsList = request.SortBy?.ToLower() switch
            {
                "timestamp" => request.SortDirection?.ToLower() == "asc" 
                    ? transactionsList.OrderBy(t => t.Timestamp).ToList()
                    : transactionsList.OrderByDescending(t => t.Timestamp).ToList(),
                "productname" => request.SortDirection?.ToLower() == "asc"
                    ? transactionsList.OrderBy(t => t.Product.Name).ToList()
                    : transactionsList.OrderByDescending(t => t.Product.Name).ToList(),
                "warehousename" => request.SortDirection?.ToLower() == "asc"
                    ? transactionsList.OrderBy(t => t.Warehouse.Name).ToList()
                    : transactionsList.OrderByDescending(t => t.Warehouse.Name).ToList(),
                "type" => request.SortDirection?.ToLower() == "asc"
                    ? transactionsList.OrderBy(t => t.TransactionType).ToList()
                    : transactionsList.OrderByDescending(t => t.TransactionType).ToList(),
                "quantity" => request.SortDirection?.ToLower() == "asc"
                    ? transactionsList.OrderBy(t => Math.Abs(t.QuantityChanged)).ToList()
                    : transactionsList.OrderByDescending(t => Math.Abs(t.QuantityChanged)).ToList(),
                _ => transactionsList.OrderByDescending(t => t.Timestamp).ToList()
            };

            var totalCount = transactionsList.Count;

            // Apply pagination
            var pagedTransactions = transactionsList
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Map to DTOs
            var transactionDtos = _mapper.Map<List<TransactionDto>>(pagedTransactions);

            _logger.LogInformation("Successfully retrieved {Count} transactions (page {PageNumber} of {TotalPages})", 
                pagedTransactions.Count, request.PageNumber, (int)Math.Ceiling((double)totalCount / request.PageSize));

            return new GetAllTransactionsQueryResponse
            {
                Transactions = transactionDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all transactions");

            return new GetAllTransactionsQueryResponse
            {
                Success = false,
                ErrorMessage = "An error occurred while retrieving transactions."
            };
        }
    }
}
