using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.WebUI.ViewModels.Report;

/// <summary>
/// ViewModel for reports dashboard with available report types
/// </summary>
public class ReportDashboardViewModel : BaseViewModel
{
    // Available Report Categories
    public List<ReportCategoryViewModel> ReportCategories { get; set; } = new();

    // Quick Statistics for Dashboard
    [Display(Name = "Total Reports Generated")]
    public int TotalReportsGenerated { get; set; }

    [Display(Name = "Most Popular Report")]
    public string MostPopularReport { get; set; } = string.Empty;

    [Display(Name = "Last Report Generated")]
    [DataType(DataType.DateTime)]
    public DateTime? LastReportDate { get; set; }

    // Recent Reports
    public List<RecentReportViewModel> RecentReports { get; set; } = new();

    public ReportDashboardViewModel()
    {
        PageTitle = "Reports Dashboard";
        PageSubtitle = "Generate and view inventory management reports";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Reports", null)
        };

        // Initialize default report categories
        ReportCategories = new List<ReportCategoryViewModel>
        {
            new()
            {
                Name = "Inventory Reports",
                Description = "Stock levels, valuations, and inventory analysis",
                Icon = "fas fa-boxes",
                Color = "primary",
                Reports = new List<ReportTypeViewModel>
                {
                    new() { Name = "Current Stock Report", Url = "/Reports/Inventory?type=Stock", Description = "View current stock levels" },
                    new() { Name = "Low Stock Report", Url = "/Reports/Inventory?type=LowStock", Description = "Products below threshold" },
                    new() { Name = "Inventory Valuation", Url = "/Reports/Inventory?type=Valuation", Description = "Total inventory value" },
                    new() { Name = "Stock Movement", Url = "/Reports/Inventory?type=Movement", Description = "Stock in/out analysis" }
                }
            },
            new()
            {
                Name = "Transaction Reports",
                Description = "Transaction history and movement analysis",
                Icon = "fas fa-exchange-alt",
                Color = "success",
                Reports = new List<ReportTypeViewModel>
                {
                    new() { Name = "Transaction History", Url = "/Report/Transaction", Description = "All inventory transactions" },
                    new() { Name = "Daily Transactions", Url = "/Report/Transaction?period=Daily", Description = "Daily transaction summary" },
                    new() { Name = "User Activity", Url = "/Report/Transaction?groupBy=User", Description = "Transactions by user" },
                    new() { Name = "Location Analysis", Url = "/Report/Transaction?groupBy=Location", Description = "Transactions by location" }
                }
            },
            new()
            {
                Name = "Product Reports",
                Description = "Product performance and category analysis",
                Icon = "fas fa-tags",
                Color = "info",
                Reports = new List<ReportTypeViewModel>
                {
                    new() { Name = "Product Performance", Url = "/Report/Product?type=Performance", Description = "Top performing products" },
                    new() { Name = "Category Analysis", Url = "/Report/Product?type=Category", Description = "Products by category" },
                    new() { Name = "Inactive Products", Url = "/Report/Product?type=Inactive", Description = "Products not recently moved" },
                    new() { Name = "Price Analysis", Url = "/Report/Product?type=Price", Description = "Product pricing analysis" }
                }
            },
            new()
            {
                Name = "System Reports",
                Description = "User activity and system usage reports",
                Icon = "fas fa-chart-bar",
                Color = "warning",
                Reports = new List<ReportTypeViewModel>
                {
                    new() { Name = "User Activity", Url = "/Report/System?type=Users", Description = "User login and activity" },
                    new() { Name = "System Usage", Url = "/Report/System?type=Usage", Description = "System usage statistics" },
                    new() { Name = "Audit Log", Url = "/Report/System?type=Audit", Description = "System audit trail" },
                    new() { Name = "Error Log", Url = "/Report/System?type=Errors", Description = "System errors and issues" }
                }
            }
        };
    }
}

/// <summary>
/// Report category for grouping related reports
/// </summary>
public class ReportCategoryViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = "fas fa-chart-bar";
    public string Color { get; set; } = "primary";
    public List<ReportTypeViewModel> Reports { get; set; } = new();
}

/// <summary>
/// Individual report type within a category
/// </summary>
public class ReportTypeViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool RequiresParameters { get; set; } = true;
    public string? PreviewImage { get; set; }
}

/// <summary>
/// Recent report for dashboard display
/// </summary>
public class RecentReportViewModel
{
    public string ReportName { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public DateTime GeneratedDate { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
    public string Parameters { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;

    public string TimeAgo
    {
        get
        {
            var timeSpan = DateTime.Now - GeneratedDate;
            if (timeSpan.TotalMinutes < 1) return "Just now";
            if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays} days ago";
            return GeneratedDate.ToString("MMM dd, yyyy");
        }
    }
}
