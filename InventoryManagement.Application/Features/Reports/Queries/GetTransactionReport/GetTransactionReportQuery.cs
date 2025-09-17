using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Reports.Queries.GetTransactionReport;

/// <summary>
/// Query to get transaction report data
/// </summary>
public class GetTransactionReportQuery : IRequest<GetTransactionReportQueryResponse>
{
    /// <summary>
    /// Report filters
    /// </summary>
    public ReportFilterDto Filter { get; set; } = new();

    /// <summary>
    /// Page number for pagination
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size for pagination
    /// </summary>
    public int PageSize { get; set; } = 50;

    /// <summary>
    /// Sort by field
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction (asc/desc)
    /// </summary>
    public string? SortDirection { get; set; }
}

/// <summary>
/// Response for transaction report query
/// </summary>
public class GetTransactionReportQueryResponse
{
    /// <summary>
    /// Transaction report items
    /// </summary>
    public List<TransactionReportDto> Items { get; set; } = new();

    /// <summary>
    /// Total count of items
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
    /// Summary statistics
    /// </summary>
    public TransactionReportSummaryDto Summary { get; set; } = new();

    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Transaction report summary statistics
/// </summary>
public class TransactionReportSummaryDto
{
    /// <summary>
    /// Total transactions in report
    /// </summary>
    public int TotalTransactions { get; set; }

    /// <summary>
    /// Total value of transactions
    /// </summary>
    public decimal TotalValue { get; set; }

    /// <summary>
    /// Stock in transactions count
    /// </summary>
    public int StockInCount { get; set; }

    /// <summary>
    /// Stock out transactions count
    /// </summary>
    public int StockOutCount { get; set; }

    /// <summary>
    /// Adjustment transactions count
    /// </summary>
    public int AdjustmentCount { get; set; }

    /// <summary>
    /// Total stock in quantity
    /// </summary>
    public int TotalStockInQuantity { get; set; }

    /// <summary>
    /// Total stock out quantity
    /// </summary>
    public int TotalStockOutQuantity { get; set; }

    /// <summary>
    /// Average transaction value
    /// </summary>
    public decimal AverageTransactionValue { get; set; }
}

/// <summary>
/// Handler for transaction report query
/// </summary>
public class GetTransactionReportQueryHandler : IRequestHandler<GetTransactionReportQuery, GetTransactionReportQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTransactionReportQueryHandler> _logger;

    public GetTransactionReportQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetTransactionReportQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetTransactionReportQueryResponse> Handle(GetTransactionReportQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting transaction report with filters");

            // Get transactions with includes
            var transactions = await _unitOfWork.Transactions.GetAsync(
                filter: BuildFilter(request.Filter),
                includeProperties: "Product,Warehouse,User",
                cancellationToken: cancellationToken);

            var transactionList = transactions.ToList();
            var totalCount = transactionList.Count;

            // Convert to DTOs
            var reportItems = transactionList.Select(t => new TransactionReportDto
            {
                TransactionId = t.Id,
                TransactionDate = t.Timestamp,
                TransactionType = t.TransactionType.ToString(),
                ProductName = t.Product?.Name ?? "Unknown",
                ProductSKU = t.Product?.SKU ?? "N/A",
                WarehouseName = t.Warehouse?.Name ?? "Unknown",
                QuantityChanged = t.QuantityChanged,
                UnitCost = t.UnitCost,
                UserName = t.User?.Username ?? "System",
                ReferenceNumber = t.ReferenceNumber,
                Reason = t.Reason,
                PreviousQuantity = t.PreviousQuantity,
                NewQuantity = t.NewQuantity
            }).ToList();

            // Apply sorting
            reportItems = ApplySorting(reportItems, request.SortBy, request.SortDirection);

            // Calculate summary statistics
            var summary = CalculateSummary(reportItems);

            // Apply pagination
            var pagedItems = reportItems
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            _logger.LogInformation("Successfully retrieved transaction report with {ItemCount} items", pagedItems.Count);

            return new GetTransactionReportQueryResponse
            {
                Items = pagedItems,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                Summary = summary,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction report");
            
            return new GetTransactionReportQueryResponse
            {
                Success = false,
                ErrorMessage = "An error occurred while retrieving the transaction report."
            };
        }
    }

    private System.Linq.Expressions.Expression<Func<Domain.Entities.Transaction, bool>>? BuildFilter(ReportFilterDto filter)
    {
        System.Linq.Expressions.Expression<Func<Domain.Entities.Transaction, bool>>? expression = null;

        // Base filter - include all active transactions
        expression = t => true;

        // Date range filter
        if (filter.StartDate.HasValue)
        {
            var startDate = filter.StartDate.Value.Date;
            var previousExpression = expression;
            expression = t => previousExpression.Compile()(t) && t.Timestamp >= startDate;
        }

        if (filter.EndDate.HasValue)
        {
            var endDate = filter.EndDate.Value.Date.AddDays(1);
            var previousExpression = expression;
            expression = t => previousExpression.Compile()(t) && t.Timestamp < endDate;
        }

        // Product filter
        if (filter.ProductIds?.Any() == true)
        {
            var previousExpression = expression;
            expression = t => previousExpression.Compile()(t) && filter.ProductIds.Contains(t.ProductId);
        }

        // Warehouse filter
        if (filter.WarehouseIds?.Any() == true)
        {
            var previousExpression = expression;
            expression = t => previousExpression.Compile()(t) && filter.WarehouseIds.Contains(t.WarehouseId);
        }

        // User filter
        if (filter.UserIds?.Any() == true)
        {
            var previousExpression = expression;
            expression = t => previousExpression.Compile()(t) && filter.UserIds.Contains(t.UserId);
        }

        // Transaction type filter
        if (filter.TransactionTypes?.Any() == true)
        {
            var transactionTypes = filter.TransactionTypes
                .Select(tt => Enum.TryParse<TransactionType>(tt, out var type) ? type : (TransactionType?)null)
                .Where(tt => tt.HasValue)
                .Select(tt => tt!.Value)
                .ToList();

            if (transactionTypes.Any())
            {
                var previousExpression = expression;
                expression = t => previousExpression.Compile()(t) && transactionTypes.Contains(t.TransactionType);
            }
        }

        return expression;
    }

    private List<TransactionReportDto> ApplySorting(List<TransactionReportDto> items, string? sortBy, string? sortDirection)
    {
        if (string.IsNullOrEmpty(sortBy))
            return items.OrderByDescending(t => t.TransactionDate).ToList();

        var isDescending = sortDirection?.ToLower() == "desc";

        return sortBy.ToLower() switch
        {
            "transactiondate" => isDescending 
                ? items.OrderByDescending(t => t.TransactionDate).ToList()
                : items.OrderBy(t => t.TransactionDate).ToList(),
            "transactiontype" => isDescending 
                ? items.OrderByDescending(t => t.TransactionType).ToList()
                : items.OrderBy(t => t.TransactionType).ToList(),
            "productname" => isDescending 
                ? items.OrderByDescending(t => t.ProductName).ToList()
                : items.OrderBy(t => t.ProductName).ToList(),
            "warehousename" => isDescending 
                ? items.OrderByDescending(t => t.WarehouseName).ToList()
                : items.OrderBy(t => t.WarehouseName).ToList(),
            "quantitychanged" => isDescending 
                ? items.OrderByDescending(t => t.QuantityChanged).ToList()
                : items.OrderBy(t => t.QuantityChanged).ToList(),
            "totalvalue" => isDescending 
                ? items.OrderByDescending(t => t.TotalValue).ToList()
                : items.OrderBy(t => t.TotalValue).ToList(),
            "username" => isDescending 
                ? items.OrderByDescending(t => t.UserName).ToList()
                : items.OrderBy(t => t.UserName).ToList(),
            _ => items.OrderByDescending(t => t.TransactionDate).ToList()
        };
    }

    private TransactionReportSummaryDto CalculateSummary(List<TransactionReportDto> items)
    {
        var totalTransactions = items.Count;
        var totalValue = items.Sum(t => t.TotalValue);
        var stockInCount = items.Count(t => t.TransactionType == "StockIn");
        var stockOutCount = items.Count(t => t.TransactionType == "StockOut");
        var adjustmentCount = items.Count(t => t.TransactionType == "Adjustment");
        var totalStockInQuantity = items.Where(t => t.TransactionType == "StockIn").Sum(t => Math.Abs(t.QuantityChanged));
        var totalStockOutQuantity = items.Where(t => t.TransactionType == "StockOut").Sum(t => Math.Abs(t.QuantityChanged));

        return new TransactionReportSummaryDto
        {
            TotalTransactions = totalTransactions,
            TotalValue = totalValue,
            StockInCount = stockInCount,
            StockOutCount = stockOutCount,
            AdjustmentCount = adjustmentCount,
            TotalStockInQuantity = totalStockInQuantity,
            TotalStockOutQuantity = totalStockOutQuantity,
            AverageTransactionValue = totalTransactions > 0 ? totalValue / totalTransactions : 0
        };
    }
}
