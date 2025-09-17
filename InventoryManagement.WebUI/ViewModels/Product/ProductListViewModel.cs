using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.ViewModels.Product;

/// <summary>
/// ViewModel for product list with search and filtering
/// </summary>
public class ProductListViewModel : PagedViewModel<ProductItemViewModel>
{
    // Search and filter properties
    [Display(Name = "Search")]
    public new string? SearchTerm { get; set; }

    [Display(Name = "Category")]
    public int? CategoryId { get; set; }

    [Display(Name = "Brand")]
    public string? Brand { get; set; }

    [Display(Name = "Status")]
    public bool? IsActive { get; set; }

    [Display(Name = "Stock Status")]
    public string? StockStatus { get; set; }

    [Display(Name = "Price Range From")]
    [DataType(DataType.Currency)]
    public decimal? PriceFrom { get; set; }

    [Display(Name = "Price Range To")]
    [DataType(DataType.Currency)]
    public decimal? PriceTo { get; set; }

    // Filter options for dropdowns
    public List<SelectListItem> Categories { get; set; } = new();
    public List<SelectListItem> Brands { get; set; } = new();
    public List<SelectListItem> StockStatuses { get; set; } = new()
    {
        new SelectListItem { Value = "", Text = "All Products" },
        new SelectListItem { Value = "InStock", Text = "In Stock" },
        new SelectListItem { Value = "LowStock", Text = "Low Stock" },
        new SelectListItem { Value = "OutOfStock", Text = "Out of Stock" }
    };

    // Summary information
    [Display(Name = "Total Products")]
    public int TotalProducts { get; set; }

    [Display(Name = "Active Products")]
    public int ActiveProducts { get; set; }

    [Display(Name = "Low Stock Products")]
    public int LowStockProducts { get; set; }

    [Display(Name = "Out of Stock Products")]
    public int OutOfStockProducts { get; set; }

    [Display(Name = "Total Inventory Value")]
    [DataType(DataType.Currency)]
    public decimal TotalInventoryValue { get; set; }

    public ProductListViewModel()
    {
        PageTitle = "Product Management";
        PageSubtitle = "Manage products and inventory items";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Products", null)
        };
    }
}

/// <summary>
/// Individual product item for list display
/// </summary>
public class ProductItemViewModel
{
    public int Id { get; set; }

    [Display(Name = "Product Name")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "SKU")]
    public string Sku { get; set; } = string.Empty;

    [Display(Name = "Category")]
    public string CategoryName { get; set; } = string.Empty;

    [Display(Name = "Brand")]
    public string? Brand { get; set; }

    [Display(Name = "Unit Price")]
    [DataType(DataType.Currency)]
    public decimal UnitPrice { get; set; }

    [Display(Name = "Current Stock")]
    public int CurrentStock { get; set; }

    [Display(Name = "Low Stock Threshold")]
    public int LowStockThreshold { get; set; }

    [Display(Name = "Status")]
    public string Status => IsActive ? "Active" : "Inactive";

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; }

    [Display(Name = "Last Modified")]
    [DataType(DataType.DateTime)]
    public DateTime? LastModified { get; set; }

    // Computed properties for display
    [Display(Name = "Stock Status")]
    public string StockStatus => CurrentStock == 0 ? "Out of Stock" : 
                                CurrentStock <= LowStockThreshold ? "Low Stock" : "In Stock";

    [Display(Name = "Stock Value")]
    [DataType(DataType.Currency)]
    public decimal StockValue => CurrentStock * UnitPrice;

    public bool IsLowStock => CurrentStock <= LowStockThreshold && CurrentStock > 0;
    public bool IsOutOfStock => CurrentStock == 0;
    public bool HasStock => CurrentStock > 0;

    // CSS classes for styling
    public string StockStatusCssClass => CurrentStock == 0 ? "badge bg-danger" :
                                        CurrentStock <= LowStockThreshold ? "badge bg-warning" : "badge bg-success";

    public string StatusCssClass => IsActive ? "badge bg-success" : "badge bg-secondary";
}
