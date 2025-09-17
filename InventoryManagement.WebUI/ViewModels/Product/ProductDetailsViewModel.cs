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
    public string SKU { get; set; } = string.Empty;

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Category")]
    public string CategoryName { get; set; } = string.Empty;

    [Display(Name = "Price")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    [Display(Name = "Cost")]
    [DataType(DataType.Currency)]
    public decimal? Cost { get; set; }

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

    [Display(Name = "Unit")]
    public string? Unit { get; set; }

    [Display(Name = "Weight (kg)")]
    public decimal? Weight { get; set; }

    [Display(Name = "Dimensions")]
    public string? Dimensions { get; set; }

    [Display(Name = "Supplier")]
    public string? SupplierName { get; set; }

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
    public decimal StockValue => CurrentStock * Price;

    [Display(Name = "Is Low Stock")]
    public bool IsLowStock => CurrentStock <= LowStockThreshold;

    // Recent inventory transactions
    public List<RecentTransactionViewModel> RecentTransactions { get; set; } = new();

    // Inventory levels by warehouse
    public List<InventoryLevelViewModel> InventoryLevels { get; set; } = new();

    // Permissions
    public bool CanEditProduct { get; set; } = true;
    public bool CanDeleteProduct { get; set; } = true;

    // Create a nested Product object for backward compatibility with views
    public ProductSummary Product => new ProductSummary
    {
        Id = this.Id,
        Name = this.Name,
        SKU = this.SKU,
        Description = this.Description,
        CategoryName = this.CategoryName,
        Price = this.Price,
        Cost = this.Cost,
        LowStockThreshold = this.LowStockThreshold,
        IsActive = this.IsActive,
        Barcode = this.Barcode,
        Brand = this.Brand,
        Unit = this.Unit,
        Weight = this.Weight,
        Dimensions = this.Dimensions,
        SupplierName = this.SupplierName,
        CreatedAt = this.CreatedDate,
        UpdatedAt = this.LastModified
    };

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
    public string Type { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

/// <summary>
/// Inventory level for product details view
/// </summary>
public class InventoryLevelViewModel
{
    public string WarehouseName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int ReservedQuantity { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Product summary for backward compatibility
/// </summary>
public class ProductSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? Cost { get; set; }
    public int LowStockThreshold { get; set; }
    public bool IsActive { get; set; }
    public string? Barcode { get; set; }
    public string? Brand { get; set; }
    public string? Unit { get; set; }
    public decimal? Weight { get; set; }
    public string? Dimensions { get; set; }
    public string? SupplierName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
