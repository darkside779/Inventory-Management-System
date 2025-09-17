using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.ViewModels.Report;

/// <summary>
/// ViewModel for transaction reports
/// </summary>
public class TransactionReportViewModel : BaseViewModel
{
    // Report Parameters
    [Display(Name = "Report Period")]
    public string ReportPeriod { get; set; } = "Monthly";

    [Display(Name = "Date From")]
    [DataType(DataType.Date)]
    public DateTime? DateFrom { get; set; } = DateTime.Now.AddMonths(-1);

    [Display(Name = "Date To")]
    [DataType(DataType.Date)]
    public DateTime? DateTo { get; set; } = DateTime.Now;

    [Display(Name = "Transaction Type")]
    public string? TransactionType { get; set; }

    [Display(Name = "Category")]
    public int? CategoryId { get; set; }

    [Display(Name = "Product")]
    public int? ProductId { get; set; }

    [Display(Name = "User")]
    public string? UserId { get; set; }

    [Display(Name = "Location")]
    public string? Location { get; set; }

    // Filter options
    public List<SelectListItem> ReportPeriods { get; set; } = new()
    {
        new SelectListItem { Value = "Daily", Text = "Daily" },
        new SelectListItem { Value = "Weekly", Text = "Weekly" },
        new SelectListItem { Value = "Monthly", Text = "Monthly" },
        new SelectListItem { Value = "Quarterly", Text = "Quarterly" },
        new SelectListItem { Value = "Yearly", Text = "Yearly" },
        new SelectListItem { Value = "Custom", Text = "Custom Range" }
    };

    public List<SelectListItem> TransactionTypes { get; set; } = new()
    {
        new SelectListItem { Value = "", Text = "All Types" },
        new SelectListItem { Value = "StockIn", Text = "Stock In" },
        new SelectListItem { Value = "StockOut", Text = "Stock Out" },
        new SelectListItem { Value = "Adjustment", Text = "Adjustment" },
        new SelectListItem { Value = "Transfer", Text = "Transfer" }
    };

    public List<SelectListItem> Categories { get; set; } = new();
    public List<SelectListItem> Products { get; set; } = new();
    public List<SelectListItem> Users { get; set; } = new();
    public List<SelectListItem> Locations { get; set; } = new();

    // Report Data
    public List<TransactionReportItemViewModel> ReportItems { get; set; } = new();

    // Summary Statistics
    [Display(Name = "Total Transactions")]
    public int TotalTransactions { get; set; }

    [Display(Name = "Stock In Quantity")]
    public int TotalStockInQuantity { get; set; }

    [Display(Name = "Stock Out Quantity")]
    public int TotalStockOutQuantity { get; set; }

    [Display(Name = "Net Stock Movement")]
    public int NetStockMovement => TotalStockInQuantity - TotalStockOutQuantity;

    [Display(Name = "Total Transaction Value")]
    [DataType(DataType.Currency)]
    public decimal TotalTransactionValue { get; set; }

    [Display(Name = "Average Transaction Value")]
    [DataType(DataType.Currency)]
    public decimal AverageTransactionValue { get; set; }

    // Chart Data
    public List<ChartDataItem> TransactionTrends { get; set; } = new();
    public List<ChartDataItem> TransactionTypeDistribution { get; set; } = new();
    public List<ChartDataItem> TopProducts { get; set; } = new();
    public List<ChartDataItem> UserActivity { get; set; } = new();

    public TransactionReportViewModel()
    {
        PageTitle = "Transaction Reports";
        PageSubtitle = "Analyze inventory transactions and movement patterns";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Reports", "/Report"),
            ("Transaction Report", null)
        };
    }
}

/// <summary>
/// Individual transaction item in report
/// </summary>
public class TransactionReportItemViewModel
{
    public int TransactionId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Location { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public decimal? UnitPrice { get; set; }
    public decimal? TransactionValue { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}
