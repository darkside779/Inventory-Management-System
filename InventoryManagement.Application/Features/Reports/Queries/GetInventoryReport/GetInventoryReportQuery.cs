using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Reports.Queries.GetInventoryReport;

/// <summary>
/// Query to get inventory report data
/// </summary>
public class GetInventoryReportQuery : IRequest<GetInventoryReportQueryResponse>
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
/// Response for inventory report query
/// </summary>
public class GetInventoryReportQueryResponse
{
    /// <summary>
    /// Inventory report items
    /// </summary>
    public List<InventoryReportDto> Items { get; set; } = new();

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
    public InventoryReportSummaryDto Summary { get; set; } = new();

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
/// Inventory report summary statistics
/// </summary>
public class InventoryReportSummaryDto
{
    /// <summary>
    /// Total items in report
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Total inventory value
    /// </summary>
    public decimal TotalValue { get; set; }

    /// <summary>
    /// Low stock items count
    /// </summary>
    public int LowStockItems { get; set; }

    /// <summary>
    /// Out of stock items count
    /// </summary>
    public int OutOfStockItems { get; set; }

    /// <summary>
    /// Overstock items count
    /// </summary>
    public int OverstockItems { get; set; }

    /// <summary>
    /// Normal stock items count
    /// </summary>
    public int NormalStockItems { get; set; }
}

/// <summary>
/// Handler for inventory report query
/// </summary>
public class GetInventoryReportQueryHandler : IRequestHandler<GetInventoryReportQuery, GetInventoryReportQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetInventoryReportQueryHandler> _logger;

    public GetInventoryReportQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetInventoryReportQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetInventoryReportQueryResponse> Handle(GetInventoryReportQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting inventory report with filters");

            // Get inventory items with includes
            var inventoryItems = await _unitOfWork.Inventory.GetAsync(
                filter: BuildFilter(request.Filter),
                includeProperties: "Product,Product.Category,Warehouse",
                cancellationToken: cancellationToken);

            var inventoryList = inventoryItems.ToList();
            var totalCount = inventoryList.Count;

            // Convert to DTOs
            var reportItems = inventoryList.Select(i => new InventoryReportDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "Unknown",
                ProductSKU = i.Product?.SKU ?? "N/A",
                CategoryName = i.Product?.Category?.Name ?? "Uncategorized",
                WarehouseId = i.WarehouseId,
                WarehouseName = i.Warehouse?.Name ?? "Unknown",
                CurrentQuantity = i.Quantity,
                ReservedQuantity = i.ReservedQuantity,
                MinimumStockLevel = i.Product?.LowStockThreshold ?? 0,
                MaximumStockLevel = (i.Product?.LowStockThreshold ?? 0) * 5, // Estimated max stock level
                UnitPrice = i.Product?.Price ?? 0,
                LastStockCount = i.LastStockCount
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

            _logger.LogInformation("Successfully retrieved inventory report with {ItemCount} items", pagedItems.Count);

            return new GetInventoryReportQueryResponse
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
            _logger.LogError(ex, "Error getting inventory report");
            
            return new GetInventoryReportQueryResponse
            {
                Success = false,
                ErrorMessage = "An error occurred while retrieving the inventory report."
            };
        }
    }

    private System.Linq.Expressions.Expression<Func<Domain.Entities.Inventory, bool>>? BuildFilter(ReportFilterDto filter)
    {
        System.Linq.Expressions.Expression<Func<Domain.Entities.Inventory, bool>>? expression = null;

        expression = i => i.IsActive;

        if (filter.ProductIds?.Any() == true)
        {
            var previousExpression = expression;
            expression = i => previousExpression.Compile()(i) && filter.ProductIds.Contains(i.ProductId);
        }

        if (filter.WarehouseIds?.Any() == true)
        {
            var previousExpression = expression;
            expression = i => previousExpression.Compile()(i) && filter.WarehouseIds.Contains(i.WarehouseId);
        }

        if (filter.LowStockOnly == true)
        {
            var previousExpression = expression;
            expression = i => previousExpression.Compile()(i) && i.Product != null && i.Quantity <= i.Product.LowStockThreshold;
        }

        if (filter.OutOfStockOnly == true)
        {
            var previousExpression = expression;
            expression = i => previousExpression.Compile()(i) && i.Quantity <= 0;
        }

        return expression;
    }

    private List<InventoryReportDto> ApplySorting(List<InventoryReportDto> items, string? sortBy, string? sortDirection)
    {
        if (string.IsNullOrEmpty(sortBy))
            return items.OrderBy(i => i.ProductName).ToList();

        var isDescending = sortDirection?.ToLower() == "desc";

        return sortBy.ToLower() switch
        {
            "productname" => isDescending 
                ? items.OrderByDescending(i => i.ProductName).ToList()
                : items.OrderBy(i => i.ProductName).ToList(),
            "categoryname" => isDescending 
                ? items.OrderByDescending(i => i.CategoryName).ToList()
                : items.OrderBy(i => i.CategoryName).ToList(),
            "warehousename" => isDescending 
                ? items.OrderByDescending(i => i.WarehouseName).ToList()
                : items.OrderBy(i => i.WarehouseName).ToList(),
            "currentquantity" => isDescending 
                ? items.OrderByDescending(i => i.CurrentQuantity).ToList()
                : items.OrderBy(i => i.CurrentQuantity).ToList(),
            "totalvalue" => isDescending 
                ? items.OrderByDescending(i => i.TotalValue).ToList()
                : items.OrderBy(i => i.TotalValue).ToList(),
            "stockstatus" => isDescending 
                ? items.OrderByDescending(i => i.StockStatus).ToList()
                : items.OrderBy(i => i.StockStatus).ToList(),
            _ => items.OrderBy(i => i.ProductName).ToList()
        };
    }

    private InventoryReportSummaryDto CalculateSummary(List<InventoryReportDto> items)
    {
        return new InventoryReportSummaryDto
        {
            TotalItems = items.Count,
            TotalValue = items.Sum(i => i.TotalValue),
            LowStockItems = items.Count(i => i.StockStatus == "Low Stock"),
            OutOfStockItems = items.Count(i => i.StockStatus == "Out of Stock"),
            OverstockItems = items.Count(i => i.StockStatus == "Overstock"),
            NormalStockItems = items.Count(i => i.StockStatus == "Normal")
        };
    }
}
