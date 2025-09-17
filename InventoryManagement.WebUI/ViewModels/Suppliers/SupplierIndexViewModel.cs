using System.ComponentModel.DataAnnotations;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.WebUI.ViewModels.Suppliers;

/// <summary>
/// ViewModel for supplier listing page
/// </summary>
public class SupplierIndexViewModel
{
    /// <summary>
    /// List of suppliers
    /// </summary>
    public IList<SupplierDto> Suppliers { get; set; } = new List<SupplierDto>();

    /// <summary>
    /// Current page number
    /// </summary>
    public int CurrentPage { get; set; } = 1;

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Total number of suppliers
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage { get; set; }

    /// <summary>
    /// Search term for filtering
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter by active suppliers only
    /// </summary>
    public bool ActiveOnly { get; set; }

    /// <summary>
    /// Filter by suppliers with products only
    /// </summary>
    public bool WithProductsOnly { get; set; }

    /// <summary>
    /// Sort field
    /// </summary>
    public string SortBy { get; set; } = "Name";

    /// <summary>
    /// Sort direction
    /// </summary>
    public string SortDirection { get; set; } = "asc";

    /// <summary>
    /// Page title
    /// </summary>
    public string PageTitle => "Suppliers";
}

/// <summary>
/// ViewModel for supplier details page
/// </summary>
public class SupplierDetailsViewModel
{
    /// <summary>
    /// Supplier ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Supplier name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Contact person
    /// </summary>
    public string? ContactPerson { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Email address
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Website URL
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// Tax identification number
    /// </summary>
    public string? TaxNumber { get; set; }

    /// <summary>
    /// Payment terms
    /// </summary>
    public string? PaymentTerms { get; set; }

    /// <summary>
    /// Is supplier active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Created date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Updated date
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Products from this supplier
    /// </summary>
    public IList<ProductDto> Products { get; set; } = new List<ProductDto>();

    /// <summary>
    /// Total number of products
    /// </summary>
    public int TotalProducts { get; set; }

    /// <summary>
    /// Number of active products
    /// </summary>
    public int ActiveProducts { get; set; }

    /// <summary>
    /// Number of inactive products
    /// </summary>
    public int InactiveProducts { get; set; }

    /// <summary>
    /// Number of low stock products
    /// </summary>
    public int LowStockProducts { get; set; }

    /// <summary>
    /// Total inventory value from this supplier
    /// </summary>
    public decimal TotalInventoryValue { get; set; }

    /// <summary>
    /// Average product price
    /// </summary>
    public decimal? AverageProductPrice { get; set; }

    /// <summary>
    /// Highest product price
    /// </summary>
    public decimal? HighestPrice { get; set; }

    /// <summary>
    /// Lowest product price
    /// </summary>
    public decimal? LowestPrice { get; set; }

    /// <summary>
    /// Page title
    /// </summary>
    public string PageTitle => $"Supplier Details - {Name}";

    /// <summary>
    /// Performance status based on products and inventory
    /// </summary>
    public string PerformanceStatus
    {
        get
        {
            if (TotalProducts == 0) return "No Products";
            if (LowStockProducts > TotalProducts * 0.5m) return "Poor";
            if (LowStockProducts > TotalProducts * 0.2m) return "Fair";
            return "Good";
        }
    }

    /// <summary>
    /// Performance status CSS class
    /// </summary>
    public string PerformanceStatusClass
    {
        get
        {
            return PerformanceStatus switch
            {
                "Good" => "text-success",
                "Fair" => "text-warning",
                "Poor" => "text-danger",
                _ => "text-muted"
            };
        }
    }
}

/// <summary>
/// ViewModel for creating new supplier
/// </summary>
public class CreateSupplierViewModel
{
    /// <summary>
    /// Supplier name
    /// </summary>
    [Required(ErrorMessage = "Supplier name is required")]
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    [Display(Name = "Supplier Name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Contact person
    /// </summary>
    [MaxLength(200, ErrorMessage = "Contact person cannot exceed 200 characters")]
    [Display(Name = "Contact Person")]
    public string? ContactPerson { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    [Display(Name = "Phone Number")]
    public string? Phone { get; set; }

    /// <summary>
    /// Email address
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    [Display(Name = "Email Address")]
    public string? Email { get; set; }

    /// <summary>
    /// Address
    /// </summary>
    [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    [Display(Name = "Address")]
    public string? Address { get; set; }

    /// <summary>
    /// Website URL
    /// </summary>
    [Url(ErrorMessage = "Invalid website URL format")]
    [MaxLength(200, ErrorMessage = "Website URL cannot exceed 200 characters")]
    [Display(Name = "Website")]
    public string? Website { get; set; }

    /// <summary>
    /// Tax identification number
    /// </summary>
    [MaxLength(50, ErrorMessage = "Tax number cannot exceed 50 characters")]
    [Display(Name = "Tax ID Number")]
    public string? TaxNumber { get; set; }

    /// <summary>
    /// Payment terms
    /// </summary>
    [MaxLength(100, ErrorMessage = "Payment terms cannot exceed 100 characters")]
    [Display(Name = "Payment Terms")]
    public string? PaymentTerms { get; set; }

    /// <summary>
    /// Is supplier active
    /// </summary>
    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Page title
    /// </summary>
    public string PageTitle => "Create New Supplier";
}

/// <summary>
/// ViewModel for editing existing supplier
/// </summary>
public class EditSupplierViewModel
{
    /// <summary>
    /// Supplier ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Supplier name
    /// </summary>
    [Required(ErrorMessage = "Supplier name is required")]
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    [Display(Name = "Supplier Name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Contact person
    /// </summary>
    [MaxLength(200, ErrorMessage = "Contact person cannot exceed 200 characters")]
    [Display(Name = "Contact Person")]
    public string? ContactPerson { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    [Display(Name = "Phone Number")]
    public string? Phone { get; set; }

    /// <summary>
    /// Email address
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    [Display(Name = "Email Address")]
    public string? Email { get; set; }

    /// <summary>
    /// Address
    /// </summary>
    [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    [Display(Name = "Address")]
    public string? Address { get; set; }

    /// <summary>
    /// Website URL
    /// </summary>
    [Url(ErrorMessage = "Invalid website URL format")]
    [MaxLength(200, ErrorMessage = "Website URL cannot exceed 200 characters")]
    [Display(Name = "Website")]
    public string? Website { get; set; }

    /// <summary>
    /// Tax identification number
    /// </summary>
    [MaxLength(50, ErrorMessage = "Tax number cannot exceed 50 characters")]
    [Display(Name = "Tax ID Number")]
    public string? TaxNumber { get; set; }

    /// <summary>
    /// Payment terms
    /// </summary>
    [MaxLength(100, ErrorMessage = "Payment terms cannot exceed 100 characters")]
    [Display(Name = "Payment Terms")]
    public string? PaymentTerms { get; set; }

    /// <summary>
    /// Is supplier active
    /// </summary>
    [Display(Name = "Active")]
    public bool IsActive { get; set; }

    /// <summary>
    /// Page title
    /// </summary>
    public string PageTitle => $"Edit Supplier - {Name}";
}

/// <summary>
/// ViewModel for deleting supplier
/// </summary>
public class DeleteSupplierViewModel
{
    /// <summary>
    /// Supplier ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Supplier name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Contact person
    /// </summary>
    public string? ContactPerson { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Email address
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Is supplier active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Created date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Updated date
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Total number of products
    /// </summary>
    public int TotalProducts { get; set; }

    /// <summary>
    /// Product count (alias for TotalProducts)
    /// </summary>
    public int ProductCount => TotalProducts;

    /// <summary>
    /// Number of active products
    /// </summary>
    public int ActiveProducts { get; set; }

    /// <summary>
    /// Total inventory value
    /// </summary>
    public decimal TotalInventoryValue { get; set; }

    /// <summary>
    /// Has active products
    /// </summary>
    public bool HasActiveProducts { get; set; }

    /// <summary>
    /// Has inventory with stock
    /// </summary>
    public bool HasInventoryWithStock { get; set; }

    /// <summary>
    /// Has recent transactions
    /// </summary>
    public bool HasRecentTransactions { get; set; }

    /// <summary>
    /// Page title
    /// </summary>
    public string PageTitle => $"Delete Supplier - {Name}";

    /// <summary>
    /// Can be deleted safely
    /// </summary>
    public bool CanDelete => !HasActiveProducts;

    /// <summary>
    /// Warning message for deletion
    /// </summary>
    public string DeletionWarning
    {
        get
        {
            if (HasActiveProducts)
                return "This supplier has active products and cannot be deleted. Please deactivate or reassign all products first.";

            if (TotalProducts > 0)
                return "This supplier has product history. Deletion will deactivate the supplier but preserve historical data.";

            return "This supplier will be permanently deactivated.";
        }
    }
}
