using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Transactions.Queries.GetTransactionsByProduct;

/// <summary>
/// Query to get transactions by product with optional filtering
/// </summary>
public class GetTransactionsByProductQuery : IRequest<GetTransactionsByProductQueryResponse>
{
    /// <summary>
    /// Product ID
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Optional warehouse ID filter
    /// </summary>
    public int? WarehouseId { get; set; }

    /// <summary>
    /// Optional transaction type filter
    /// </summary>
    public Domain.Enums.TransactionType? TransactionType { get; set; }

    /// <summary>
    /// Start date for filtering
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date for filtering
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 20;

    public GetTransactionsByProductQuery(int productId)
    {
        ProductId = productId;
    }
}

/// <summary>
/// Response for GetTransactionsByProductQuery
/// </summary>
public class GetTransactionsByProductQueryResponse
{
    /// <summary>
    /// Product information
    /// </summary>
    public ProductDto? Product { get; set; }

    /// <summary>
    /// List of transactions for the product
    /// </summary>
    public IEnumerable<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();

    /// <summary>
    /// Transaction statistics
    /// </summary>
    public TransactionStatistics Statistics { get; set; } = new();

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
    /// Indicates if the product was found
    /// </summary>
    public bool IsFound { get; set; }

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
/// Transaction statistics for a product
/// </summary>
public class TransactionStatistics
{
    /// <summary>
    /// Total stock in quantity
    /// </summary>
    public int TotalStockIn { get; set; }

    /// <summary>
    /// Total stock out quantity
    /// </summary>
    public int TotalStockOut { get; set; }

    /// <summary>
    /// Total adjustments quantity (can be positive or negative)
    /// </summary>
    public int TotalAdjustments { get; set; }

    /// <summary>
    /// Net movement (stock in - stock out + adjustments)
    /// </summary>
    public int NetMovement => TotalStockIn - TotalStockOut + TotalAdjustments;

    /// <summary>
    /// Total value of stock in transactions
    /// </summary>
    public decimal TotalStockInValue { get; set; }

    /// <summary>
    /// Total value of stock out transactions
    /// </summary>
    public decimal TotalStockOutValue { get; set; }

    /// <summary>
    /// Average unit cost for stock in transactions
    /// </summary>
    public decimal? AverageStockInCost { get; set; }

    /// <summary>
    /// Number of stock in transactions
    /// </summary>
    public int StockInTransactionCount { get; set; }

    /// <summary>
    /// Number of stock out transactions
    /// </summary>
    public int StockOutTransactionCount { get; set; }

    /// <summary>
    /// Number of adjustment transactions
    /// </summary>
    public int AdjustmentTransactionCount { get; set; }

    /// <summary>
    /// Total number of transactions
    /// </summary>
    public int TotalTransactionCount => StockInTransactionCount + StockOutTransactionCount + AdjustmentTransactionCount;
}

/// <summary>
/// Handler for GetTransactionsByProductQuery
/// </summary>
public class GetTransactionsByProductQueryHandler : IRequestHandler<GetTransactionsByProductQuery, GetTransactionsByProductQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTransactionsByProductQueryHandler> _logger;

    public GetTransactionsByProductQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetTransactionsByProductQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetTransactionsByProductQueryResponse> Handle(GetTransactionsByProductQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting transactions for product ID: {ProductId}", request.ProductId);

            // First, verify the product exists
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken: cancellationToken);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", request.ProductId);
                
                return new GetTransactionsByProductQueryResponse
                {
                    IsFound = false,
                    Success = true
                };
            }

            // Get transactions for the product
            var transactions = await _unitOfWork.Transactions.GetAsync(
                filter: t => 
                    t.ProductId == request.ProductId &&
                    (!request.WarehouseId.HasValue || t.WarehouseId == request.WarehouseId.Value) &&
                    (!request.TransactionType.HasValue || t.TransactionType == request.TransactionType.Value) &&
                    (!request.StartDate.HasValue || t.Timestamp >= request.StartDate.Value) &&
                    (!request.EndDate.HasValue || t.Timestamp <= request.EndDate.Value),
                includeProperties: "Product,Warehouse,User",
                orderBy: q => q.OrderByDescending(t => t.Timestamp),
                cancellationToken: cancellationToken);

            var transactionsList = transactions.ToList();
            var totalCount = transactionsList.Count;

            // Calculate statistics
            var statistics = CalculateTransactionStatistics(transactionsList);

            // Apply pagination
            var pagedTransactions = transactionsList
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Map to DTOs
            var productDto = _mapper.Map<ProductDto>(product);
            var transactionDtos = _mapper.Map<List<TransactionDto>>(pagedTransactions);

            _logger.LogInformation("Successfully retrieved {Count} transactions for product ID: {ProductId} (page {PageNumber})", 
                pagedTransactions.Count, request.ProductId, request.PageNumber);

            return new GetTransactionsByProductQueryResponse
            {
                Product = productDto,
                Transactions = transactionDtos,
                Statistics = statistics,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                IsFound = true,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting transactions for product ID: {ProductId}", request.ProductId);

            return new GetTransactionsByProductQueryResponse
            {
                IsFound = false,
                Success = false,
                ErrorMessage = "An error occurred while retrieving product transactions."
            };
        }
    }

    private static TransactionStatistics CalculateTransactionStatistics(List<Domain.Entities.Transaction> transactions)
    {
        var statistics = new TransactionStatistics();

        foreach (var transaction in transactions)
        {
            switch (transaction.TransactionType)
            {
                case Domain.Enums.TransactionType.StockIn:
                    statistics.TotalStockIn += Math.Abs(transaction.QuantityChanged);
                    statistics.StockInTransactionCount++;
                    if (transaction.UnitCost.HasValue)
                    {
                        statistics.TotalStockInValue += Math.Abs(transaction.QuantityChanged) * transaction.UnitCost.Value;
                    }
                    break;

                case Domain.Enums.TransactionType.StockOut:
                    statistics.TotalStockOut += Math.Abs(transaction.QuantityChanged);
                    statistics.StockOutTransactionCount++;
                    if (transaction.UnitCost.HasValue)
                    {
                        statistics.TotalStockOutValue += Math.Abs(transaction.QuantityChanged) * transaction.UnitCost.Value;
                    }
                    break;

                case Domain.Enums.TransactionType.Adjustment:
                    statistics.TotalAdjustments += transaction.QuantityChanged; // Keep sign for adjustments
                    statistics.AdjustmentTransactionCount++;
                    break;
            }
        }

        // Calculate average stock in cost
        if (statistics.StockInTransactionCount > 0 && statistics.TotalStockIn > 0)
        {
            statistics.AverageStockInCost = statistics.TotalStockInValue / statistics.TotalStockIn;
        }

        return statistics;
    }
}
