using System.ComponentModel.DataAnnotations;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Extensions;

namespace InventoryManagement.WebUI.ViewModels.Products;

/// <summary>
/// ViewModel for product listing page
/// </summary>
public class ProductIndexViewModel : BaseViewModel
{
    /// <summary>
    /// Paginated product results
    /// </summary>
    public PagedResult<ProductDto> Products { get; set; } = new();

    /// <summary>
    /// Current search term
    /// </summary>
    public string? CurrentSearch { get; set; }

    /// <summary>
    /// Current category filter
    /// </summary>
    public int? CurrentCategoryId { get; set; }

    /// <summary>
    /// Available categories for filtering
    /// </summary>
    public List<CategoryDto> Categories { get; set; } = new();

    /// <summary>
    /// Show product creation button
    /// </summary>
    public bool CanCreateProducts => true;

    /// <summary>
    /// Show product edit/delete buttons
    /// </summary>
    public bool CanManageProducts => true;

    public ProductIndexViewModel()
    {
        PageTitle = "Products";
        PageSubtitle = "Manage your product catalog";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Products", null)
        };
    }
}

/// <summary>
/// ViewModel for product details page
/// </summary>
public class ProductDetailsViewModel : BaseViewModel
{
    /// <summary>
    /// Product information
    /// </summary>
    public ProductDto Product { get; set; } = null!;

    /// <summary>
    /// Current inventory levels across warehouses
    /// </summary>
    public List<InventoryDto> InventoryLevels { get; set; } = new();

    /// <summary>
    /// Recent transactions for this product
    /// </summary>
    public List<TransactionDto> RecentTransactions { get; set; } = new();

    /// <summary>
    /// Show edit button
    /// </summary>
    public bool CanEditProduct => true;

    /// <summary>
    /// Show delete button
    /// </summary>
    public bool CanDeleteProduct => true;

    public ProductDetailsViewModel()
    {
        PageTitle = "Product Details";
        PageSubtitle = "View product information and inventory";
    }

    public void SetProduct(ProductDto product)
    {
        Product = product;
        PageTitle = product.Name;
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Products", "/Product"),
            (product.Name, null)
        };
    }
}

/// <summary>
/// Base class for product form ViewModels
/// </summary>
public abstract class ProductFormViewModelBase : BaseViewModel
{
    [Display(Name = "Product Name")]
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "SKU")]
    [Required(ErrorMessage = "SKU is required")]
    [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters")]
    public string SKU { get; set; } = string.Empty;

    [Display(Name = "Description")]
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Display(Name = "Price")]
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    [Display(Name = "Cost")]
    [Range(0, double.MaxValue, ErrorMessage = "Cost cannot be negative")]
    [DataType(DataType.Currency)]
    public decimal? Cost { get; set; }

    [Display(Name = "Category")]
    [Required(ErrorMessage = "Category is required")]
    public int CategoryId { get; set; }

    [Display(Name = "Supplier")]
    public int? SupplierId { get; set; }

    [Display(Name = "Low Stock Threshold")]
    [Required(ErrorMessage = "Low stock threshold is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Threshold cannot be negative")]
    public int LowStockThreshold { get; set; } = 10;

    [Display(Name = "Unit")]
    [Required(ErrorMessage = "Unit is required")]
    [StringLength(20, ErrorMessage = "Unit cannot exceed 20 characters")]
    public string Unit { get; set; } = "Piece";

    [Display(Name = "Barcode")]
    [StringLength(100, ErrorMessage = "Barcode cannot exceed 100 characters")]
    public string? Barcode { get; set; }

    [Display(Name = "Weight (kg)")]
    [Range(0, double.MaxValue, ErrorMessage = "Weight cannot be negative")]
    public decimal? Weight { get; set; }

    [Display(Name = "Dimensions")]
    [StringLength(100, ErrorMessage = "Dimensions cannot exceed 100 characters")]
    public string? Dimensions { get; set; }

    /// <summary>
    /// Available categories for dropdown
    /// </summary>
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Categories { get; set; } = new();

    /// <summary>
    /// Available suppliers for dropdown
    /// </summary>
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Suppliers { get; set; } = new();
}

/// <summary>
/// ViewModel for creating new products
/// </summary>
public class CreateProductViewModel : ProductFormViewModelBase
{
    public CreateProductViewModel()
    {
        PageTitle = "Create Product";
        PageSubtitle = "Add a new product to your catalog";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Products", "/Product"),
            ("Create", null)
        };
    }
}

/// <summary>
/// ViewModel for editing existing products
/// </summary>
public class EditProductViewModel : ProductFormViewModelBase
{
    /// <summary>
    /// Product ID
    /// </summary>
    public int Id { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Created")]
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Last Updated")]
    [DataType(DataType.DateTime)]
    public DateTime UpdatedAt { get; set; }

    public EditProductViewModel()
    {
        PageTitle = "Edit Product";
        PageSubtitle = "Update product information";
    }

    public void SetProduct(ProductDto product)
    {
        Id = product.Id;
        Name = product.Name;
        SKU = product.SKU;
        Description = product.Description;
        Price = product.Price;
        Cost = product.Cost;
        CategoryId = product.CategoryId;
        SupplierId = product.SupplierId;
        LowStockThreshold = product.LowStockThreshold;
        Unit = product.Unit;
        Barcode = product.Barcode;
        Weight = product.Weight;
        Dimensions = product.Dimensions;
        IsActive = product.IsActive;
        CreatedAt = product.CreatedAt;
        UpdatedAt = product.UpdatedAt;

        PageTitle = $"Edit {product.Name}";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Products", "/Product"),
            (product.Name, $"/Product/Details/{product.Id}"),
            ("Edit", null)
        };
    }
}

/// <summary>
/// ViewModel for product deletion confirmation
/// </summary>
public class DeleteProductViewModel : BaseViewModel
{
    /// <summary>
    /// Product to be deleted
    /// </summary>
    public ProductDto Product { get; set; } = null!;

    /// <summary>
    /// Whether product has inventory records
    /// </summary>
    public bool HasInventoryRecords { get; set; }

    /// <summary>
    /// Warning message for deletion
    /// </summary>
    public string WarningMessage => HasInventoryRecords 
        ? "This product has inventory records and will be marked as inactive instead of being permanently deleted."
        : "This action cannot be undone. The product will be permanently removed from the system.";

    public DeleteProductViewModel()
    {
        PageTitle = "Delete Product";
        PageSubtitle = "Confirm product deletion";
    }

    public void SetProduct(ProductDto product)
    {
        Product = product;
        PageTitle = $"Delete {product.Name}";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Products", "/Product"),
            (product.Name, $"/Product/Details/{product.Id}"),
            ("Delete", null)
        };
    }
}
