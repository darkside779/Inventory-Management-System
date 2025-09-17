using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.ViewModels.Product;

/// <summary>
/// ViewModel for creating new products
/// </summary>
public class ProductCreateViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
    [Display(Name = "Product Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "SKU is required")]
    [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters")]
    [Display(Name = "SKU")]
    public string SKU { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    [Display(Name = "Price")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Cost must be 0 or greater")]
    [Display(Name = "Cost")]
    [DataType(DataType.Currency)]
    public decimal? Cost { get; set; }

    [Required(ErrorMessage = "Low stock threshold is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Low stock threshold must be 0 or greater")]
    [Display(Name = "Low Stock Threshold")]
    public int LowStockThreshold { get; set; } = 10;

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    [StringLength(100, ErrorMessage = "Barcode cannot exceed 100 characters")]
    [Display(Name = "Barcode")]
    public string? Barcode { get; set; }

    [StringLength(100, ErrorMessage = "Brand cannot exceed 100 characters")]
    [Display(Name = "Brand")]
    public string? Brand { get; set; }

    [StringLength(50, ErrorMessage = "Unit cannot exceed 50 characters")]
    [Display(Name = "Unit")]
    public string? Unit { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Weight must be 0 or greater")]
    [Display(Name = "Weight (kg)")]
    public decimal? Weight { get; set; }

    [StringLength(200, ErrorMessage = "Dimensions cannot exceed 200 characters")]
    [Display(Name = "Dimensions (L×W×H)")]
    public string? Dimensions { get; set; }

    [Display(Name = "Supplier")]
    public int? SupplierId { get; set; }

    // Navigation properties for dropdowns
    public List<SelectListItem> Categories { get; set; } = new();
    public List<SelectListItem> Suppliers { get; set; } = new();

    public ProductCreateViewModel()
    {
        PageTitle = "Add New Product";
        PageSubtitle = "Create a new product in the inventory system";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Products", "/Product"),
            ("Add Product", null)
        };
    }
}
