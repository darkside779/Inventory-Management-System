using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.WebUI.ViewModels.Product;

/// <summary>
/// ViewModel for confirming product deletion
/// </summary>
public class DeleteProductViewModel : BaseViewModel
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

    [Display(Name = "Unit")]
    public string? Unit { get; set; }

    [Display(Name = "Weight (kg)")]
    public decimal? Weight { get; set; }

    [Display(Name = "Dimensions")]
    public string? Dimensions { get; set; }

    [Display(Name = "Supplier")]
    public string? SupplierName { get; set; }

    [Display(Name = "Created Date")]
    [DataType(DataType.DateTime)]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Last Modified")]
    [DataType(DataType.DateTime)]
    public DateTime? LastModified { get; set; }

    [Display(Name = "Current Stock")]
    public int CurrentStock { get; set; }

    [Display(Name = "Stock Status")]
    public string StockStatus => CurrentStock <= LowStockThreshold ? "Low Stock" : "In Stock";

    [Display(Name = "Stock Value")]
    [DataType(DataType.Currency)]
    public decimal StockValue => CurrentStock * Price;

    // Additional properties for delete confirmation
    public bool HasInventoryRecords => CurrentStock > 0;
    public string? WarningMessage => HasInventoryRecords ? 
        "Warning: This product has inventory records. Deleting it will remove all associated inventory data." : null;

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
        Unit = this.Unit,
        Weight = this.Weight,
        Dimensions = this.Dimensions,
        SupplierName = this.SupplierName
    };

    [Display(Name = "Is Low Stock")]
    public bool IsLowStock => CurrentStock <= LowStockThreshold;

    public DeleteProductViewModel()
    {
        PageTitle = "Delete Product";
        PageSubtitle = "Confirm product deletion";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Products", "/Product"),
            ("Delete Product", null)
        };
    }
}
