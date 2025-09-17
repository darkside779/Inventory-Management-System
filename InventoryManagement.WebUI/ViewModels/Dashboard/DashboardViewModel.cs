using System.ComponentModel.DataAnnotations;
using InventoryManagement.WebUI.ViewModels;

namespace InventoryManagement.WebUI.ViewModels.Dashboard;

/// <summary>
/// Main dashboard ViewModel with overview statistics and recent activities
/// </summary>
public class DashboardViewModel : BaseViewModel
{
    // Key Performance Indicators
    [Display(Name = "Total Products")]
    public int TotalProducts { get; set; }

    [Display(Name = "Total Categories")]
    public int TotalCategories { get; set; }

    [Display(Name = "Total Inventory Value")]
    [DataType(DataType.Currency)]
    public decimal TotalInventoryValue { get; set; }

    [Display(Name = "Low Stock Products")]
    public int LowStockProducts { get; set; }

    [Display(Name = "Out of Stock Products")]
    public int OutOfStockProducts { get; set; }

    [Display(Name = "Active Users")]
    public int ActiveUsers { get; set; }

    // Recent Activity Summaries
    [Display(Name = "Today's Transactions")]
    public int TodaysTransactions { get; set; }

    [Display(Name = "This Week's Transactions")]
    public int WeeklyTransactions { get; set; }

    [Display(Name = "This Month's Transactions")]
    public int MonthlyTransactions { get; set; }

    [Display(Name = "Stock In (This Month)")]
    public int MonthlyStockIn { get; set; }

    [Display(Name = "Stock Out (This Month)")]
    public int MonthlyStockOut { get; set; }

    // Charts and Trends Data
    public List<ChartDataPoint> StockMovementChart { get; set; } = new();
    public List<ChartDataPoint> CategoryDistributionChart { get; set; } = new();
    public List<ChartDataPoint> MonthlyTransactionTrends { get; set; } = new();

    // Recent Activities
    public List<RecentActivityViewModel> RecentActivities { get; set; } = new();

    // Low Stock Alerts
    public List<LowStockAlertViewModel> LowStockAlerts { get; set; } = new();

    // Top Products
    public List<TopProductViewModel> TopMovingProducts { get; set; } = new();
    public List<TopProductViewModel> HighestValueProducts { get; set; } = new();

    // Quick Actions Available
    public bool CanAddProducts { get; set; }
    public bool CanManageInventory { get; set; }
    public bool CanViewReports { get; set; }
    public bool CanManageUsers { get; set; }

    public DashboardViewModel()
    {
        PageTitle = "Dashboard";
        PageSubtitle = "Inventory Management Overview";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Dashboard", null)
        };
    }
}

/// <summary>
/// Chart data point for dashboard charts
/// </summary>
public class ChartDataPoint
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string? Color { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Recent activity item for dashboard
/// </summary>
public class RecentActivityViewModel
{
    public int Id { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime ActivityDate { get; set; }
    public string? ProductName { get; set; }
    public string? CategoryName { get; set; }
    public string Icon { get; set; } = "fas fa-info-circle";
    public string BadgeClass { get; set; } = "badge bg-info";

    public string TimeAgo
    {
        get
        {
            var timeSpan = DateTime.Now - ActivityDate;
            if (timeSpan.TotalMinutes < 1) return "Just now";
            if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays} days ago";
            return ActivityDate.ToString("MMM dd, yyyy");
        }
    }
}

/// <summary>
/// Low stock alert for dashboard
/// </summary>
public class LowStockAlertViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int LowStockThreshold { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime LastRestocked { get; set; }

    public string AlertLevel => CurrentStock == 0 ? "Critical" : "Warning";
    public string AlertClass => CurrentStock == 0 ? "alert alert-danger" : "alert alert-warning";
    public string BadgeClass => CurrentStock == 0 ? "badge bg-danger" : "badge bg-warning";
}

/// <summary>
/// Top product for dashboard statistics
/// </summary>
public class TopProductViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Value { get; set; }
    public int Rank { get; set; }
}
