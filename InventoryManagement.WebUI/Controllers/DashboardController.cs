using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using AutoMapper;
using InventoryManagement.Application.Features.Dashboard.Queries.GetDashboardData;
using InventoryManagement.WebUI.ViewModels.Dashboard;

namespace InventoryManagement.WebUI.Controllers;

/// <summary>
/// Controller for dashboard functionality and main system overview
/// </summary>
[Authorize]
public class DashboardController : BaseController
{
    private readonly IMapper _mapper;

    public DashboardController(IMediator mediator, ILogger<DashboardController> logger, IMapper mapper) 
        : base(mediator, logger)
    {
        _mapper = mapper;
    }

    /// <summary>
    /// Main dashboard index with KPIs and overview data
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        try
        {
            SetPageTitle("Dashboard", "Inventory Management System Overview");
            SetBreadcrumb(("Dashboard", null));

            // Create a simple dashboard for now
            var viewModel = new DashboardViewModel
            {
                TotalProducts = 0,
                TotalCategories = 0,
                TotalInventoryValue = 0,
                LowStockProducts = 0,
                OutOfStockProducts = 0,
                ActiveUsers = 1,
                TodaysTransactions = 0,
                WeeklyTransactions = 0,
                MonthlyTransactions = 0,
                MonthlyStockIn = 0,
                MonthlyStockOut = 0,
                CanAddProducts = IsManagerOrAdmin,
                CanManageInventory = IsManagerOrAdmin,
                CanViewReports = IsManagerOrAdmin,
                CanManageUsers = IsAdmin
            };

            LogUserAction("Accessed Dashboard");
            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading dashboard");
        }
    }

    /// <summary>
    /// API endpoint for chart data - Category Distribution
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCategoryDistributionData()
    {
        try
        {
            var query = new GetDashboardDataQuery
            {
                UserId = GetCurrentUserId()
            };

            var dashboardData = await _mediator.Send(query);
            
            return JsonSuccess("Category distribution data retrieved", new
            {
                labels = dashboardData.CategoryDistributionChart.Select(c => c.Label).ToArray(),
                data = dashboardData.CategoryDistributionChart.Select(c => c.Value).ToArray(),
                backgroundColor = dashboardData.CategoryDistributionChart.Select(c => c.Color ?? "#007bff").ToArray()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category distribution data for user {UserId}", GetCurrentUserId());
            return JsonError("Failed to load category distribution data");
        }
    }

    /// <summary>
    /// API endpoint for chart data - Monthly Transaction Trends
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetTransactionTrendsData()
    {
        try
        {
            var query = new GetDashboardDataQuery
            {
                UserId = GetCurrentUserId()
            };

            var dashboardData = await _mediator.Send(query);
            
            return JsonSuccess("Transaction trends data retrieved", new
            {
                labels = dashboardData.MonthlyTransactionTrends.Select(t => t.Label).ToArray(),
                data = dashboardData.MonthlyTransactionTrends.Select(t => t.Value).ToArray(),
                borderColor = "#28a745",
                backgroundColor = "rgba(40, 167, 69, 0.1)",
                fill = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction trends data for user {UserId}", GetCurrentUserId());
            return JsonError("Failed to load transaction trends data");
        }
    }

    /// <summary>
    /// API endpoint for refreshing recent activities
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetRecentActivities()
    {
        try
        {
            var query = new GetDashboardDataQuery
            {
                UserId = GetCurrentUserId()
            };

            var dashboardData = await _mediator.Send(query);
            
            return JsonSuccess("Recent activities retrieved", dashboardData.RecentActivities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent activities for user {UserId}", GetCurrentUserId());
            return JsonError("Failed to load recent activities");
        }
    }

    /// <summary>
    /// API endpoint for refreshing low stock alerts
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetLowStockAlerts()
    {
        try
        {
            var query = new GetDashboardDataQuery
            {
                UserId = GetCurrentUserId()
            };

            var dashboardData = await _mediator.Send(query);
            
            return JsonSuccess("Low stock alerts retrieved", dashboardData.LowStockAlerts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting low stock alerts for user {UserId}", GetCurrentUserId());
            return JsonError("Failed to load low stock alerts");
        }
    }

    /// <summary>
    /// API endpoint for dashboard statistics summary
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetDashboardStats()
    {
        try
        {
            var query = new GetDashboardDataQuery
            {
                UserId = GetCurrentUserId()
            };

            var dashboardData = await _mediator.Send(query);
            
            return JsonSuccess("Dashboard statistics retrieved", new
            {
                totalProducts = dashboardData.TotalProducts,
                totalCategories = dashboardData.TotalCategories,
                totalInventoryValue = dashboardData.TotalInventoryValue,
                lowStockProducts = dashboardData.LowStockProducts,
                outOfStockProducts = dashboardData.OutOfStockProducts,
                todaysTransactions = dashboardData.TodaysTransactions,
                weeklyTransactions = dashboardData.WeeklyTransactions,
                monthlyTransactions = dashboardData.MonthlyTransactions,
                monthlyStockIn = dashboardData.MonthlyStockIn,
                monthlyStockOut = dashboardData.MonthlyStockOut
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard stats for user {UserId}", GetCurrentUserId());
            return JsonError("Failed to load dashboard statistics");
        }
    }

    /// <summary>
    /// Quick action: Navigate to add new product
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "ManagerOnly")]
    public IActionResult QuickAddProduct()
    {
        LogUserAction("Quick Action - Add Product");
        return RedirectToAction("Create", "Product");
    }

    /// <summary>
    /// Quick action: Navigate to stock in
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "ManagerOnly")]
    public IActionResult QuickStockIn()
    {
        LogUserAction("Quick Action - Stock In");
        return RedirectToAction("StockIn", "Inventory");
    }

    /// <summary>
    /// Quick action: Navigate to stock out
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "ManagerOnly")]
    public IActionResult QuickStockOut()
    {
        LogUserAction("Quick Action - Stock Out");
        return RedirectToAction("StockOut", "Inventory");
    }

    /// <summary>
    /// Quick action: Navigate to low stock report
    /// </summary>
    [HttpPost]
    public IActionResult QuickLowStockReport()
    {
        LogUserAction("Quick Action - Low Stock Report");
        return RedirectToAction("Inventory", "Report", new { type = "LowStock" });
    }
}
