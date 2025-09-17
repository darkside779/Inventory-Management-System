using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Reports.Queries.GetDashboardSummary;

/// <summary>
/// Query to get dashboard summary data
/// </summary>
public class GetDashboardSummaryQuery : IRequest<GetDashboardSummaryQueryResponse>
{
}

/// <summary>
/// Response for dashboard summary query
/// </summary>
public class GetDashboardSummaryQueryResponse
{
    /// <summary>
    /// Dashboard summary data
    /// </summary>
    public DashboardSummaryDto Summary { get; set; } = new();

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
/// Handler for dashboard summary query
/// </summary>
public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, GetDashboardSummaryQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetDashboardSummaryQueryHandler> _logger;

    public GetDashboardSummaryQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetDashboardSummaryQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<GetDashboardSummaryQueryResponse> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting dashboard summary data");

            var today = DateTime.UtcNow.Date;
            var monthStart = new DateTime(today.Year, today.Month, 1);

            // Get all products
            var products = await _unitOfWork.Products.GetAllAsync(cancellationToken);
            var totalProducts = products.Count();

            // Get all active inventory items
            var inventoryItems = await _unitOfWork.Inventory.GetAsync(
                filter: i => i.IsActive,
                includeProperties: "Product",
                cancellationToken: cancellationToken);

            var totalInventoryItems = inventoryItems.Count();

            // Calculate total inventory value
            var totalInventoryValue = inventoryItems.Sum(i => i.Quantity * (i.Product?.Price ?? 0));

            // Get low stock items
            var lowStockItems = inventoryItems.Where(i => i.Product != null && i.Quantity <= i.Product.LowStockThreshold).Count();

            // Get out of stock items
            var outOfStockItems = inventoryItems.Where(i => i.Quantity <= 0).Count();

            // Get transactions for today
            var todayTransactions = await _unitOfWork.Transactions.GetAsync(
                filter: t => t.Timestamp >= today && t.Timestamp < today.AddDays(1),
                cancellationToken: cancellationToken);

            var transactionsToday = todayTransactions.Count();

            // Get transactions for this month
            var monthlyTransactions = await _unitOfWork.Transactions.GetAsync(
                filter: t => t.Timestamp >= monthStart,
                cancellationToken: cancellationToken);

            var transactionsThisMonth = monthlyTransactions.Count();

            // Get stock in/out transactions today
            var stockInToday = todayTransactions.Count(t => t.TransactionType == TransactionType.StockIn);
            var stockOutToday = todayTransactions.Count(t => t.TransactionType == TransactionType.StockOut);

            // Calculate stock movement value today
            var stockMovementValueToday = todayTransactions.Sum(t => Math.Abs(t.QuantityChanged) * (t.UnitCost ?? 0));

            var summary = new DashboardSummaryDto
            {
                TotalProducts = totalProducts,
                TotalInventoryItems = totalInventoryItems,
                TotalInventoryValue = totalInventoryValue,
                LowStockItems = lowStockItems,
                OutOfStockItems = outOfStockItems,
                TransactionsToday = transactionsToday,
                TransactionsThisMonth = transactionsThisMonth,
                StockInToday = stockInToday,
                StockOutToday = stockOutToday,
                StockMovementValueToday = stockMovementValueToday
            };

            _logger.LogInformation("Successfully retrieved dashboard summary");

            return new GetDashboardSummaryQueryResponse
            {
                Summary = summary,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard summary");
            
            return new GetDashboardSummaryQueryResponse
            {
                Success = false,
                ErrorMessage = "An error occurred while retrieving dashboard summary."
            };
        }
    }
}
