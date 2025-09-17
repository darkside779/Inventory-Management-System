using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.ViewModels.Product;

/// <summary>
/// ViewModel for editing existing products
/// </summary>
public class ProductEditViewModel : BaseViewModel
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
    [Display(Name = "Product Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "SKU is required")]
    [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters")]
    [Display(Name = "SKU")]
    public string Sku { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Unit price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
    [Display(Name = "Unit Price")]
    [DataType(DataType.Currency)]
    public decimal UnitPrice { get; set; }

    [Required(ErrorMessage = "Low stock threshold is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Low stock threshold must be 0 or greater")]
    [Display(Name = "Low Stock Threshold")]
    public int LowStockThreshold { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; }

    [StringLength(100, ErrorMessage = "Barcode cannot exceed 100 characters")]
    [Display(Name = "Barcode")]
    public string? Barcode { get; set; }

    [StringLength(100, ErrorMessage = "Brand cannot exceed 100 characters")]
    [Display(Name = "Brand")]
    public string? Brand { get; set; }

    [StringLength(50, ErrorMessage = "Unit of measure cannot exceed 50 characters")]
    [Display(Name = "Unit of Measure")]
    public string? UnitOfMeasure { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Weight must be 0 or greater")]
    [Display(Name = "Weight (kg)")]
    public decimal? Weight { get; set; }

    [StringLength(200, ErrorMessage = "Dimensions cannot exceed 200 characters")]
    [Display(Name = "Dimensions (L×W×H)")]
    public string? Dimensions { get; set; }

    [StringLength(100, ErrorMessage = "Supplier cannot exceed 100 characters")]
    [Display(Name = "Supplier")]
    public string? Supplier { get; set; }

    // Read-only properties for display
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

    // Current stock information
    [Display(Name = "Current Stock")]
    public int CurrentStock { get; set; }

    [Display(Name = "Stock Status")]
    public string StockStatus => CurrentStock <= LowStockThreshold ? "Low Stock" : "In Stock";

    // Navigation properties for dropdowns
    public List<SelectListItem> Categories { get; set; } = new();
    public List<SelectListItem> UnitsOfMeasure { get; set; } = new();

    public ProductEditViewModel()
    {
        PageTitle = "Edit Product";
        PageSubtitle = "Update product information";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Products", "/Product"),
            ("Edit Product", null)
        };
    }
}
