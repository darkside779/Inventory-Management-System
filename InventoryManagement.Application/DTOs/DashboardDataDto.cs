namespace InventoryManagement.Application.DTOs;

/// <summary>
/// Data transfer object for dashboard information
/// </summary>
public class DashboardDataDto
{
    // Key Performance Indicators
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public int LowStockProducts { get; set; }
    public int OutOfStockProducts { get; set; }
    public int ActiveUsers { get; set; }

    // Recent Activity Summaries
    public int TodaysTransactions { get; set; }
    public int WeeklyTransactions { get; set; }
    public int MonthlyTransactions { get; set; }
    public int MonthlyStockIn { get; set; }
    public int MonthlyStockOut { get; set; }

    // Chart Data
    public List<ChartDataPointDto> StockMovementChart { get; set; } = new();
    public List<ChartDataPointDto> CategoryDistributionChart { get; set; } = new();
    public List<ChartDataPointDto> MonthlyTransactionTrends { get; set; } = new();

    // Recent Activities and Alerts
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
    public List<LowStockAlertDto> LowStockAlerts { get; set; } = new();
    public List<TopProductDto> TopMovingProducts { get; set; } = new();
    public List<TopProductDto> HighestValueProducts { get; set; } = new();
}

/// <summary>
/// Chart data point for dashboard visualizations
/// </summary>
public class ChartDataPointDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string? Color { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Recent activity item for dashboard
/// </summary>
public class RecentActivityDto
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
}

/// <summary>
/// Low stock alert for dashboard
/// </summary>
public class LowStockAlertDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int LowStockThreshold { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime LastRestocked { get; set; }
    public string AlertLevel { get; set; } = string.Empty;
}

/// <summary>
/// Top product for dashboard statistics
/// </summary>
public class TopProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Value { get; set; }
    public int Rank { get; set; }
}
