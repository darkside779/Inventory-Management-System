using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.WebUI.ViewModels.Product;

/// <summary>
/// ViewModel for displaying product details
/// </summary>
public class ProductDetailsViewModel : BaseViewModel
{
    public int Id { get; set; }

    [Display(Name = "Product Name")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "SKU")]
    public string Sku { get; set; } = string.Empty;

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Category")]
    public string CategoryName { get; set; } = string.Empty;

    [Display(Name = "Unit Price")]
    [DataType(DataType.Currency)]
    public decimal UnitPrice { get; set; }

    [Display(Name = "Low Stock Threshold")]
    public int LowStockThreshold { get; set; }

    [Display(Name = "Status")]
    public string Status => IsActive ? "Active" : "Inactive";

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; }

    [Display(Name = "Barcode")]
    public string? Barcode { get; set; }

    [Display(Name = "Brand")]
    public string? Brand { get; set; }

    [Display(Name = "Unit of Measure")]
    public string? UnitOfMeasure { get; set; }

    [Display(Name = "Weight (kg)")]
    public decimal? Weight { get; set; }

    [Display(Name = "Dimensions")]
    public string? Dimensions { get; set; }

    [Display(Name = "Supplier")]
    public string? Supplier { get; set; }

    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;

    [Display(Name = "Created Date")]
    [DataType(DataType.DateTime)]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Last Modified")]
    [DataType(DataType.DateTime)]
    public DateTime? LastModified { get; set; }

    [Display(Name = "Modified By")]
    public string? ModifiedBy { get; set; }

    // Stock information
    [Display(Name = "Current Stock")]
    public int CurrentStock { get; set; }

    [Display(Name = "Stock Status")]
    public string StockStatus => CurrentStock <= LowStockThreshold ? "Low Stock" : "In Stock";

    [Display(Name = "Stock Value")]
    [DataType(DataType.Currency)]
    public decimal StockValue => CurrentStock * UnitPrice;

    [Display(Name = "Is Low Stock")]
    public bool IsLowStock => CurrentStock <= LowStockThreshold;

    // Recent inventory transactions
    public List<RecentTransactionViewModel> RecentTransactions { get; set; } = new();

    // Summary statistics
    [Display(Name = "Total Stock In (30 days)")]
    public int TotalStockIn30Days { get; set; }

    [Display(Name = "Total Stock Out (30 days)")]
    public int TotalStockOut30Days { get; set; }

    [Display(Name = "Net Movement (30 days)")]
    public int NetMovement30Days => TotalStockIn30Days - TotalStockOut30Days;

    [Display(Name = "Average Monthly Usage")]
    public decimal AverageMonthlyUsage { get; set; }

    [Display(Name = "Estimated Days Until Stockout")]
    public int? EstimatedDaysUntilStockout => 
        AverageMonthlyUsage > 0 ? (int?)(CurrentStock / (AverageMonthlyUsage / 30)) : null;

    public ProductDetailsViewModel()
    {
        PageTitle = "Product Details";
        PageSubtitle = "View product information and stock levels";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Products", "/Product"),
            ("Product Details", null)
        };
    }
}

/// <summary>
/// Recent transaction for product details view
/// </summary>
public class RecentTransactionViewModel
{
    public int Id { get; set; }
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Location { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
}
