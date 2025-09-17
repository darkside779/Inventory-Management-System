using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Features.Reports.Queries.GetInventoryReport;
using InventoryManagement.Application.Features.Reports.Queries.GetTransactionReport;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.WebUI.ViewModels.Reports;

/// <summary>
/// Dashboard view model
/// </summary>
public class DashboardViewModel
{
    /// <summary>
    /// Dashboard summary data
    /// </summary>
    public DashboardSummaryDto Summary { get; set; } = new();

    /// <summary>
    /// Last refresh time
    /// </summary>
    public DateTime RefreshTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Chart data for stock levels trend
    /// </summary>
    public List<StockLevelTrendDto> StockTrend { get; set; } = new();

    /// <summary>
    /// Chart data for transaction activity
    /// </summary>
    public List<object> TransactionActivity { get; set; } = new();

    /// <summary>
    /// Top products by movement
    /// </summary>
    public List<ProductMovementReportDto> TopProducts { get; set; } = new();

    /// <summary>
    /// Warehouse performance data
    /// </summary>
    public List<WarehousePerformanceReportDto> WarehousePerformance { get; set; } = new();
}

/// <summary>
/// Base filter view model for reports
/// </summary>
public abstract class BaseReportFilterViewModel
{
    /// <summary>
    /// Selected product IDs
    /// </summary>
    public List<int>? ProductIds { get; set; }

    /// <summary>
    /// Selected warehouse IDs
    /// </summary>
    public List<int>? WarehouseIds { get; set; }

    /// <summary>
    /// Selected category IDs
    /// </summary>
    public List<int>? CategoryIds { get; set; }

    /// <summary>
    /// Minimum value filter
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Minimum value must be greater than or equal to 0")]
    public decimal? MinValue { get; set; }

    /// <summary>
    /// Maximum value filter
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Maximum value must be greater than or equal to 0")]
    public decimal? MaxValue { get; set; }

    /// <summary>
    /// Available products for selection
    /// </summary>
    public SelectList? Products { get; set; }

    /// <summary>
    /// Available warehouses for selection
    /// </summary>
    public SelectList? Warehouses { get; set; }

    /// <summary>
    /// Available categories for selection
    /// </summary>
    public SelectList? Categories { get; set; }
}

/// <summary>
/// Inventory report filter view model
/// </summary>
public class InventoryReportFilterViewModel : BaseReportFilterViewModel
{
    /// <summary>
    /// Show only low stock items
    /// </summary>
    public bool LowStockOnly { get; set; }

    /// <summary>
    /// Show only out of stock items
    /// </summary>
    public bool OutOfStockOnly { get; set; }

    /// <summary>
    /// Stock status filter options
    /// </summary>
    public SelectList StockStatusOptions { get; set; } = new SelectList(new[]
    {
        new { Value = "", Text = "All Status" },
        new { Value = "Normal", Text = "Normal Stock" },
        new { Value = "Low Stock", Text = "Low Stock" },
        new { Value = "Out of Stock", Text = "Out of Stock" },
        new { Value = "Overstock", Text = "Overstock" }
    }, "Value", "Text");
}

/// <summary>
/// Transaction report filter view model
/// </summary>
public class TransactionReportFilterViewModel : BaseReportFilterViewModel
{
    /// <summary>
    /// Start date for filtering
    /// </summary>
    [Display(Name = "Start Date")]
    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date for filtering
    /// </summary>
    [Display(Name = "End Date")]
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Selected user IDs
    /// </summary>
    public List<int>? UserIds { get; set; }

    /// <summary>
    /// Selected transaction types
    /// </summary>
    public List<string>? TransactionTypes { get; set; }

    /// <summary>
    /// Available users for selection
    /// </summary>
    public SelectList? Users { get; set; }

    /// <summary>
    /// Available transaction types for selection
    /// </summary>
    public SelectList TransactionTypeOptions { get; set; } = new SelectList(new[]
    {
        new { Value = "StockIn", Text = "Stock In" },
        new { Value = "StockOut", Text = "Stock Out" },
        new { Value = "Adjustment", Text = "Adjustment" }
    }, "Value", "Text");
}

/// <summary>
/// Product movement report filter view model
/// </summary>
public class ProductMovementReportFilterViewModel : BaseReportFilterViewModel
{
    /// <summary>
    /// Start date for analysis period
    /// </summary>
    [Display(Name = "Analysis Start Date")]
    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; } = DateTime.UtcNow.AddMonths(-3);

    /// <summary>
    /// End date for analysis period
    /// </summary>
    [Display(Name = "Analysis End Date")]
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Minimum transaction count filter
    /// </summary>
    [Display(Name = "Minimum Transactions")]
    [Range(0, int.MaxValue, ErrorMessage = "Minimum transaction count must be 0 or greater")]
    public int? MinTransactions { get; set; }

    /// <summary>
    /// Movement type filter
    /// </summary>
    [Display(Name = "Movement Type")]
    public string? MovementType { get; set; }

    /// <summary>
    /// Movement type options
    /// </summary>
    public SelectList MovementTypeOptions { get; set; } = new SelectList(new[]
    {
        new { Value = "", Text = "All Movement" },
        new { Value = "High", Text = "High Activity" },
        new { Value = "Medium", Text = "Medium Activity" },
        new { Value = "Low", Text = "Low Activity" },
        new { Value = "None", Text = "No Activity" }
    }, "Value", "Text");
}

/// <summary>
/// Base report view model
/// </summary>
public abstract class BaseReportViewModel
{
    /// <summary>
    /// Current page number
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 25;

    /// <summary>
    /// Total count of items
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Current sort field
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Current sort direction
    /// </summary>
    public string? SortDirection { get; set; }

    /// <summary>
    /// Whether to show the first page link
    /// </summary>
    public bool ShowFirst => PageNumber > 1;

    /// <summary>
    /// Whether to show the previous page link
    /// </summary>
    public bool ShowPrevious => PageNumber > 1;

    /// <summary>
    /// Whether to show the next page link
    /// </summary>
    public bool ShowNext => PageNumber < TotalPages;

    /// <summary>
    /// Whether to show the last page link
    /// </summary>
    public bool ShowLast => PageNumber < TotalPages;

    /// <summary>
    /// Get sort class for column header
    /// </summary>
    public string GetSortClass(string column)
    {
        if (SortBy?.Equals(column, StringComparison.OrdinalIgnoreCase) == true)
        {
            return SortDirection?.ToLower() == "desc" ? "fas fa-sort-down" : "fas fa-sort-up";
        }
        return "fas fa-sort text-muted";
    }

    /// <summary>
    /// Get next sort direction for column
    /// </summary>
    public string GetNextSortDirection(string column)
    {
        if (SortBy?.Equals(column, StringComparison.OrdinalIgnoreCase) == true)
        {
            return SortDirection?.ToLower() == "desc" ? "asc" : "desc";
        }
        return "asc";
    }
}

/// <summary>
/// Inventory report view model
/// </summary>
public class InventoryReportViewModel : BaseReportViewModel
{
    /// <summary>
    /// Report items
    /// </summary>
    public List<InventoryReportDto> Items { get; set; } = new();

    /// <summary>
    /// Report summary statistics
    /// </summary>
    public InventoryReportSummaryDto Summary { get; set; } = new();

    /// <summary>
    /// Filter parameters
    /// </summary>
    public InventoryReportFilterViewModel Filter { get; set; } = new();

    /// <summary>
    /// Export formats
    /// </summary>
    public SelectList ExportFormats { get; set; } = new SelectList(new[]
    {
        new { Value = "csv", Text = "CSV" },
        new { Value = "excel", Text = "Excel" },
        new { Value = "pdf", Text = "PDF" }
    }, "Value", "Text");
}

/// <summary>
/// Transaction report view model
/// </summary>
public class TransactionReportViewModel : BaseReportViewModel
{
    /// <summary>
    /// Report items
    /// </summary>
    public List<TransactionReportDto> Items { get; set; } = new();

    /// <summary>
    /// Report summary statistics
    /// </summary>
    public TransactionReportSummaryDto Summary { get; set; } = new();

    /// <summary>
    /// Filter parameters
    /// </summary>
    public TransactionReportFilterViewModel Filter { get; set; } = new();

    /// <summary>
    /// Export formats
    /// </summary>
    public SelectList ExportFormats { get; set; } = new SelectList(new[]
    {
        new { Value = "csv", Text = "CSV" },
        new { Value = "excel", Text = "Excel" },
        new { Value = "pdf", Text = "PDF" }
    }, "Value", "Text");
}

/// <summary>
/// Product movement report view model
/// </summary>
public class ProductMovementReportViewModel : BaseReportViewModel
{
    /// <summary>
    /// Report items
    /// </summary>
    public List<ProductMovementReportDto> Items { get; set; } = new();

    /// <summary>
    /// Filter parameters
    /// </summary>
    public ProductMovementReportFilterViewModel Filter { get; set; } = new();

    /// <summary>
    /// Export formats
    /// </summary>
    public SelectList ExportFormats { get; set; } = new SelectList(new[]
    {
        new { Value = "csv", Text = "CSV" },
        new { Value = "excel", Text = "Excel" },
        new { Value = "pdf", Text = "PDF" }
    }, "Value", "Text");

    /// <summary>
    /// Chart data for movement trends
    /// </summary>
    public List<object> MovementTrendData { get; set; } = new();

    /// <summary>
    /// Chart data for top movers
    /// </summary>
    public List<object> TopMoversData { get; set; } = new();
}

/// <summary>
/// Chart configuration for reports
/// </summary>
public class ChartConfigViewModel
{
    /// <summary>
    /// Chart title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Chart type (line, bar, pie, etc.)
    /// </summary>
    public string Type { get; set; } = "line";

    /// <summary>
    /// Chart data
    /// </summary>
    public object Data { get; set; } = new();

    /// <summary>
    /// Chart options
    /// </summary>
    public object Options { get; set; } = new();

    /// <summary>
    /// Chart container ID
    /// </summary>
    public string ContainerId { get; set; } = string.Empty;
}

/// <summary>
/// KPI card view model
/// </summary>
public class KpiCardViewModel
{
    /// <summary>
    /// Card title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Main value
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Value format (number, currency, percentage)
    /// </summary>
    public string Format { get; set; } = "number";

    /// <summary>
    /// Change indicator
    /// </summary>
    public decimal? Change { get; set; }

    /// <summary>
    /// Change period (vs yesterday, vs last month, etc.)
    /// </summary>
    public string? ChangePeriod { get; set; }

    /// <summary>
    /// Icon class
    /// </summary>
    public string Icon { get; set; } = "fas fa-chart-line";

    /// <summary>
    /// Card color theme
    /// </summary>
    public string ColorClass { get; set; } = "primary";

    /// <summary>
    /// Additional description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Link URL for drill-down
    /// </summary>
    public string? LinkUrl { get; set; }

    /// <summary>
    /// Whether change is positive (green) or negative (red)
    /// </summary>
    public bool IsChangePositive => Change >= 0;

    /// <summary>
    /// Formatted change text
    /// </summary>
    public string ChangeText
    {
        get
        {
            if (!Change.HasValue) return string.Empty;
            var sign = Change >= 0 ? "+" : "";
            return $"{sign}{Change:F1}%";
        }
    }
}
