using MediatR;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Application.Features.Dashboard.Queries.GetDashboardData;

/// <summary>
/// Handler for getting comprehensive dashboard data
/// </summary>
public class GetDashboardDataQueryHandler : IRequestHandler<GetDashboardDataQuery, DashboardDataDto>
{
    private readonly IApplicationDbContext _context;

    public GetDashboardDataQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDataDto> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.Now;
        var today = now.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var monthStart = new DateTime(now.Year, now.Month, 1);

        // Get basic counts
        var totalProducts = await _context.Products.CountAsync(cancellationToken);
        var totalCategories = await _context.Categories.CountAsync(cancellationToken);
        var activeUsers = await _context.Users.CountAsync(cancellationToken);

        // Get low stock and out of stock products
        var lowStockProducts = await _context.Products
            .Where(p => p.IsActive)
            .SelectMany(p => _context.Inventories
                .Where(i => i.ProductId == p.Id)
                .GroupBy(i => i.ProductId)
                .Select(g => new { ProductId = g.Key, TotalStock = g.Sum(i => i.Quantity) }))
            .Where(stock => _context.Products.Any(p => p.Id == stock.ProductId && stock.TotalStock <= p.LowStockThreshold))
            .CountAsync(cancellationToken);

        var outOfStockProducts = await _context.Products
            .Where(p => p.IsActive)
            .SelectMany(p => _context.Inventories
                .Where(i => i.ProductId == p.Id)
                .GroupBy(i => i.ProductId)
                .Select(g => new { ProductId = g.Key, TotalStock = g.Sum(i => i.Quantity) }))
            .Where(stock => stock.TotalStock == 0)
            .CountAsync(cancellationToken);

        // Calculate total inventory value
        var totalInventoryValue = await _context.Products
            .Where(p => p.IsActive)
            .SelectMany(p => _context.Inventories
                .Where(i => i.ProductId == p.Id)
                .GroupBy(i => i.ProductId)
                .Select(g => new { ProductId = g.Key, TotalStock = g.Sum(i => i.Quantity) }))
            .Join(_context.Products, stock => stock.ProductId, p => p.Id, 
                (stock, p) => stock.TotalStock * p.Price)
            .SumAsync(cancellationToken);

        // Get transaction counts
        var todaysTransactions = await _context.Transactions
            .Where(t => t.CreatedAt.Date == today)
            .CountAsync(cancellationToken);

        var weeklyTransactions = await _context.Transactions
            .Where(t => t.CreatedAt >= weekStart)
            .CountAsync(cancellationToken);

        var monthlyTransactions = await _context.Transactions
            .Where(t => t.CreatedAt >= monthStart)
            .CountAsync(cancellationToken);

        var monthlyStockIn = await _context.Transactions
            .Where(t => t.CreatedAt >= monthStart && t.TransactionType == TransactionType.StockIn)
            .SumAsync(t => Math.Abs(t.QuantityChanged), cancellationToken);

        var monthlyStockOut = await _context.Transactions
            .Where(t => t.CreatedAt >= monthStart && t.TransactionType == TransactionType.StockOut)
            .SumAsync(t => Math.Abs(t.QuantityChanged), cancellationToken);

        // Get recent activities
        var recentActivities = await _context.Transactions
            .Include(t => t.Product)
            .Include(t => t.Product.Category)
            .Include(t => t.User)
            .OrderByDescending(t => t.CreatedAt)
            .Take(10)
            .Select(t => new RecentActivityDto
            {
                Id = t.Id,
                ActivityType = t.TransactionType == TransactionType.StockIn ? "Stock In" : "Stock Out",
                Description = $"{t.TransactionType} - {t.Product.Name} ({Math.Abs(t.QuantityChanged)} units)",
                UserName = t.User != null ? t.User.FullName : "System",
                ActivityDate = t.CreatedAt,
                ProductName = t.Product.Name,
                CategoryName = t.Product.Category.Name,
                Icon = t.TransactionType == TransactionType.StockIn ? "fas fa-arrow-up" : "fas fa-arrow-down",
                BadgeClass = t.TransactionType == TransactionType.StockIn ? "badge bg-success" : "badge bg-danger"
            })
            .ToListAsync(cancellationToken);

        // Get low stock alerts - simplified approach to avoid EF Core translation issues
        var products = await _context.Products
            .Where(p => p.IsActive)
            .Include(p => p.Category)
            .Include(p => p.InventoryItems)
            .ToListAsync(cancellationToken);

        var lowStockAlerts = products
            .Select(p => new LowStockAlertDto
            {
                ProductId = p.Id,
                ProductName = p.Name,
                ProductSku = p.SKU,
                CategoryName = p.Category?.Name ?? "Unknown",
                CurrentStock = p.InventoryItems?.Sum(i => i.Quantity) ?? 0,
                LowStockThreshold = p.LowStockThreshold,
                UnitPrice = p.Price,
                LastRestocked = p.InventoryItems?.Any() == true ? p.InventoryItems.Max(i => i.UpdatedAt) : p.CreatedAt,
                AlertLevel = (p.InventoryItems?.Sum(i => i.Quantity) ?? 0) == 0 ? "Critical" : "Warning"
            })
            .Where(x => x.CurrentStock <= x.LowStockThreshold)
            .OrderBy(x => x.CurrentStock)
            .Take(10)
            .ToList();

        // Get category distribution chart data
        var categoryDistribution = await _context.Categories
            .Where(c => c.IsActive)
            .Select(c => new ChartDataPointDto
            {
                Label = c.Name,
                Value = _context.Products.Count(p => p.CategoryId == c.Id && p.IsActive),
                Color = "#007bff"
            })
            .Where(c => c.Value > 0)
            .ToListAsync(cancellationToken);

        // Get monthly transaction trends (last 6 months)
        var monthlyTrends = new List<ChartDataPointDto>();
        for (int i = 5; i >= 0; i--)
        {
            var month = now.AddMonths(-i);
            var monthName = month.ToString("MMM yyyy");
            var monthTransactions = await _context.Transactions
                .Where(t => t.CreatedAt.Year == month.Year && t.CreatedAt.Month == month.Month)
                .CountAsync(cancellationToken);
            
            monthlyTrends.Add(new ChartDataPointDto
            {
                Label = monthName,
                Value = monthTransactions,
                Color = "#28a745"
            });
        }

        // Get top moving products (by transaction volume)
        var topMovingProducts = await _context.Transactions
            .Where(t => t.CreatedAt >= monthStart)
            .Include(t => t.Product)
            .Include(t => t.Product.Category)
            .GroupBy(t => t.Product)
            .Select(g => new TopProductDto
            {
                ProductId = g.Key.Id,
                ProductName = g.Key.Name,
                ProductSku = g.Key.SKU,
                CategoryName = g.Key.Category.Name,
                Quantity = g.Sum(t => Math.Abs(t.QuantityChanged)),
                Value = g.Sum(t => Math.Abs(t.QuantityChanged)) * g.Key.Price
            })
            .OrderByDescending(p => p.Quantity)
            .Take(5)
            .ToListAsync(cancellationToken);

        // Add ranking
        for (int i = 0; i < topMovingProducts.Count; i++)
        {
            topMovingProducts[i].Rank = i + 1;
        }

        // Get highest value products - simplified approach
        var productsWithInventory = await _context.Products
            .Where(p => p.IsActive)
            .Include(p => p.Category)
            .Include(p => p.InventoryItems)
            .ToListAsync(cancellationToken);

        var highestValueProducts = productsWithInventory
            .Select(p => new TopProductDto
            {
                ProductId = p.Id,
                ProductName = p.Name,
                ProductSku = p.SKU,
                CategoryName = p.Category?.Name ?? "Unknown",
                Quantity = p.InventoryItems?.Sum(i => i.Quantity) ?? 0,
                Value = (p.InventoryItems?.Sum(i => i.Quantity) ?? 0) * p.Price
            })
            .OrderByDescending(p => p.Value)
            .Take(5)
            .ToList();

        // Add ranking
        for (int i = 0; i < highestValueProducts.Count; i++)
        {
            highestValueProducts[i].Rank = i + 1;
        }

        return new DashboardDataDto
        {
            TotalProducts = totalProducts,
            TotalCategories = totalCategories,
            TotalInventoryValue = totalInventoryValue,
            LowStockProducts = lowStockProducts,
            OutOfStockProducts = outOfStockProducts,
            ActiveUsers = activeUsers,
            TodaysTransactions = todaysTransactions,
            WeeklyTransactions = weeklyTransactions,
            MonthlyTransactions = monthlyTransactions,
            MonthlyStockIn = monthlyStockIn,
            MonthlyStockOut = monthlyStockOut,
            CategoryDistributionChart = categoryDistribution,
            MonthlyTransactionTrends = monthlyTrends,
            RecentActivities = recentActivities,
            LowStockAlerts = lowStockAlerts,
            TopMovingProducts = topMovingProducts,
            HighestValueProducts = highestValueProducts
        };
    }
}
