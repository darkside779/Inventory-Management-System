using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Reports.Queries.GetProductMovementReport;

/// <summary>
/// Query to get product movement analysis report
/// </summary>
public class GetProductMovementReportQuery : IRequest<GetProductMovementReportQueryResponse>
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
/// Response for product movement report query
/// </summary>
public class GetProductMovementReportQueryResponse
{
    /// <summary>
    /// Product movement report items
    /// </summary>
    public List<ProductMovementReportDto> Items { get; set; } = new();

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
    /// Whether the operation was successful
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Handler for product movement report query
/// </summary>
public class GetProductMovementReportQueryHandler : IRequestHandler<GetProductMovementReportQuery, GetProductMovementReportQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductMovementReportQueryHandler> _logger;

    public GetProductMovementReportQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetProductMovementReportQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetProductMovementReportQueryResponse> Handle(GetProductMovementReportQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting product movement report with filters");

            // Get all products
            var products = await _unitOfWork.Products.GetAsync(
                includeProperties: "Category",
                cancellationToken: cancellationToken);

            var productList = products.ToList();

            // Apply product filters
            if (request.Filter.ProductIds?.Any() == true)
            {
                productList = productList.Where(p => request.Filter.ProductIds.Contains(p.Id)).ToList();
            }

            if (request.Filter.CategoryIds?.Any() == true)
            {
                productList = productList.Where(p => request.Filter.CategoryIds.Contains(p.CategoryId)).ToList();
            }

            // Get transactions for analysis
            var startDate = request.Filter.StartDate ?? DateTime.UtcNow.AddMonths(-3);
            var endDate = request.Filter.EndDate ?? DateTime.UtcNow;

            var transactions = await _unitOfWork.Transactions.GetAsync(
                filter: t => t.Timestamp >= startDate && t.Timestamp <= endDate,
                includeProperties: "Product,Product.Category",
                cancellationToken: cancellationToken);

            var transactionList = transactions.ToList();

            // Build report items
            var reportItems = new List<ProductMovementReportDto>();

            foreach (var product in productList)
            {
                var productTransactions = transactionList.Where(t => t.ProductId == product.Id).ToList();

                if (!productTransactions.Any() && request.Filter.StartDate.HasValue)
                    continue; // Skip products with no transactions in the filtered period

                // Calculate metrics
                var stockInTransactions = productTransactions.Where(t => t.TransactionType == TransactionType.StockIn).ToList();
                var stockOutTransactions = productTransactions.Where(t => t.TransactionType == TransactionType.StockOut).ToList();

                var totalStockIn = stockInTransactions.Sum(t => Math.Abs(t.QuantityChanged));
                var totalStockOut = stockOutTransactions.Sum(t => Math.Abs(t.QuantityChanged));
                var totalStockInValue = stockInTransactions.Sum(t => Math.Abs(t.QuantityChanged) * (t.UnitCost ?? 0));
                var totalStockOutValue = stockOutTransactions.Sum(t => Math.Abs(t.QuantityChanged) * (t.UnitCost ?? 0));

                // Get current inventory
                var currentInventory = await _unitOfWork.Inventory.GetAsync(
                    filter: i => i.ProductId == product.Id,
                    cancellationToken: cancellationToken);

                var currentStock = currentInventory.Sum(i => i.Quantity);

                var movementReport = new ProductMovementReportDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductSKU = product.SKU,
                    CategoryName = product.Category?.Name ?? "Uncategorized",
                    TotalStockIn = totalStockIn,
                    TotalStockOut = totalStockOut,
                    TotalStockInValue = totalStockInValue,
                    TotalStockOutValue = totalStockOutValue,
                    TransactionCount = productTransactions.Count,
                    FirstTransactionDate = productTransactions.OrderBy(t => t.Timestamp).FirstOrDefault()?.Timestamp,
                    LastTransactionDate = productTransactions.OrderByDescending(t => t.Timestamp).FirstOrDefault()?.Timestamp,
                    CurrentStockLevel = currentStock
                };

                reportItems.Add(movementReport);
            }

            var totalCount = reportItems.Count;

            // Apply sorting
            reportItems = ApplySorting(reportItems, request.SortBy, request.SortDirection);

            // Apply pagination
            var pagedItems = reportItems
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            _logger.LogInformation("Successfully retrieved product movement report with {ItemCount} items", pagedItems.Count);

            return new GetProductMovementReportQueryResponse
            {
                Items = pagedItems,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product movement report");
            
            return new GetProductMovementReportQueryResponse
            {
                Success = false,
                ErrorMessage = "An error occurred while retrieving the product movement report."
            };
        }
    }

    private List<ProductMovementReportDto> ApplySorting(List<ProductMovementReportDto> items, string? sortBy, string? sortDirection)
    {
        if (string.IsNullOrEmpty(sortBy))
            return items.OrderBy(p => p.ProductName).ToList();

        var isDescending = sortDirection?.ToLower() == "desc";

        return sortBy.ToLower() switch
        {
            "productname" => isDescending 
                ? items.OrderByDescending(p => p.ProductName).ToList()
                : items.OrderBy(p => p.ProductName).ToList(),
            "categoryname" => isDescending 
                ? items.OrderByDescending(p => p.CategoryName).ToList()
                : items.OrderBy(p => p.CategoryName).ToList(),
            "totalstockin" => isDescending 
                ? items.OrderByDescending(p => p.TotalStockIn).ToList()
                : items.OrderBy(p => p.TotalStockIn).ToList(),
            "totalstockout" => isDescending 
                ? items.OrderByDescending(p => p.TotalStockOut).ToList()
                : items.OrderBy(p => p.TotalStockOut).ToList(),
            "netmovement" => isDescending 
                ? items.OrderByDescending(p => p.NetMovement).ToList()
                : items.OrderBy(p => p.NetMovement).ToList(),
            "transactioncount" => isDescending 
                ? items.OrderByDescending(p => p.TransactionCount).ToList()
                : items.OrderBy(p => p.TransactionCount).ToList(),
            "movementvelocity" => isDescending 
                ? items.OrderByDescending(p => p.MovementVelocity).ToList()
                : items.OrderBy(p => p.MovementVelocity).ToList(),
            "currentstocklevel" => isDescending 
                ? items.OrderByDescending(p => p.CurrentStockLevel).ToList()
                : items.OrderBy(p => p.CurrentStockLevel).ToList(),
            _ => items.OrderBy(p => p.ProductName).ToList()
        };
    }
}
