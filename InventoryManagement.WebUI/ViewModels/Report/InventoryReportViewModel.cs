using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.ViewModels.Report;

/// <summary>
/// ViewModel for inventory reports
/// </summary>
public class InventoryReportViewModel : BaseViewModel
{
    // Report Parameters
    [Display(Name = "Report Type")]
    public string ReportType { get; set; } = "Stock";

    [Display(Name = "Date From")]
    [DataType(DataType.Date)]
    public DateTime? DateFrom { get; set; } = DateTime.Now.AddMonths(-1);

    [Display(Name = "Date To")]
    [DataType(DataType.Date)]
    public DateTime? DateTo { get; set; } = DateTime.Now;

    [Display(Name = "Category")]
    public int? CategoryId { get; set; }

    [Display(Name = "Product")]
    public int? ProductId { get; set; }

    [Display(Name = "Location")]
    public string? Location { get; set; }

    [Display(Name = "Include Inactive Products")]
    public bool IncludeInactive { get; set; }

    // Filter options
    public List<SelectListItem> ReportTypes { get; set; } = new()
    {
        new SelectListItem { Value = "Stock", Text = "Current Stock Report" },
        new SelectListItem { Value = "LowStock", Text = "Low Stock Report" },
        new SelectListItem { Value = "StockMovement", Text = "Stock Movement Report" },
        new SelectListItem { Value = "Valuation", Text = "Inventory Valuation Report" },
        new SelectListItem { Value = "Transactions", Text = "Transaction History Report" }
    };

    public List<SelectListItem> Categories { get; set; } = new();
    public List<SelectListItem> Products { get; set; } = new();
    public List<SelectListItem> Locations { get; set; } = new();

    // Report Data
    public List<InventoryReportItemViewModel> ReportItems { get; set; } = new();

    // Summary Statistics
    [Display(Name = "Total Products")]
    public int TotalProducts { get; set; }

    [Display(Name = "Total Stock Value")]
    [DataType(DataType.Currency)]
    public decimal TotalStockValue { get; set; }

    [Display(Name = "Low Stock Items")]
    public int LowStockItems { get; set; }

    [Display(Name = "Out of Stock Items")]
    public int OutOfStockItems { get; set; }

    [Display(Name = "Average Stock Value")]
    [DataType(DataType.Currency)]
    public decimal AverageStockValue { get; set; }

    // Chart Data
    public List<ChartDataItem> CategoryDistribution { get; set; } = new();
    public List<ChartDataItem> StockStatusDistribution { get; set; } = new();

    public InventoryReportViewModel()
    {
        PageTitle = "Inventory Reports";
        PageSubtitle = "Generate comprehensive inventory analysis reports";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Reports", "/Report"),
            ("Inventory Report", null)
        };
    }
}

/// <summary>
/// Individual item in inventory report
/// </summary>
public class InventoryReportItemViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int LowStockThreshold { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal StockValue { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public string StockStatus { get; set; } = string.Empty;
    public int StockIn30Days { get; set; }
    public int StockOut30Days { get; set; }
    public int NetMovement30Days { get; set; }
}

/// <summary>
/// Chart data item for reports
/// </summary>
public class ChartDataItem
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public int Count { get; set; }
    public string Color { get; set; } = string.Empty;
}
