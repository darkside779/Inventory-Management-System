using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.ViewModels.Search;

/// <summary>
/// ViewModel for advanced filtering across different entity types
/// </summary>
public class AdvancedFilterViewModel : BaseViewModel
{
    // Common Filter Properties
    [Display(Name = "Entity Type")]
    public string EntityType { get; set; } = "Product";

    [Display(Name = "Search Term")]
    [StringLength(100, ErrorMessage = "Search term cannot exceed 100 characters")]
    public string? SearchTerm { get; set; }

    [Display(Name = "Date From")]
    [DataType(DataType.Date)]
    public DateTime? DateFrom { get; set; }

    [Display(Name = "Date To")]
    [DataType(DataType.Date)]
    public DateTime? DateTo { get; set; }

    [Display(Name = "Status")]
    public string? Status { get; set; }

    [Display(Name = "Created By")]
    public string? CreatedBy { get; set; }

    // Product-specific filters
    [Display(Name = "Category")]
    public int? CategoryId { get; set; }

    [Display(Name = "Price Range From")]
    [DataType(DataType.Currency)]
    public decimal? PriceFrom { get; set; }

    [Display(Name = "Price Range To")]  
    [DataType(DataType.Currency)]
    public decimal? PriceTo { get; set; }

    [Display(Name = "Stock Status")]
    public string? StockStatus { get; set; }

    [Display(Name = "Brand")]
    public string? Brand { get; set; }

    // Transaction-specific filters
    [Display(Name = "Transaction Type")]
    public string? TransactionType { get; set; }

    [Display(Name = "Location")]
    public string? Location { get; set; }

    [Display(Name = "Quantity From")]
    public int? QuantityFrom { get; set; }

    [Display(Name = "Quantity To")]
    public int? QuantityTo { get; set; }

    // User-specific filters
    [Display(Name = "Role")]
    public string? Role { get; set; }

    [Display(Name = "Department")]
    public string? Department { get; set; }

    [Display(Name = "Last Login From")]
    [DataType(DataType.Date)]
    public DateTime? LastLoginFrom { get; set; }

    [Display(Name = "Last Login To")]
    [DataType(DataType.Date)]
    public DateTime? LastLoginTo { get; set; }

    // Sorting options
    [Display(Name = "Sort By")]
    public string SortBy { get; set; } = "Name";

    [Display(Name = "Sort Direction")]
    public string SortDirection { get; set; } = "Ascending";

    // Pagination
    [Display(Name = "Page Size")]
    [Range(10, 100, ErrorMessage = "Page size must be between 10 and 100")]
    public int PageSize { get; set; } = 25;

    // Filter options for dropdowns
    public List<SelectListItem> EntityTypes { get; set; } = new()
    {
        new SelectListItem { Value = "Product", Text = "Products" },
        new SelectListItem { Value = "Category", Text = "Categories" },
        new SelectListItem { Value = "Transaction", Text = "Transactions" },
        new SelectListItem { Value = "User", Text = "Users" }
    };

    public List<SelectListItem> Categories { get; set; } = new();
    public List<SelectListItem> Brands { get; set; } = new();
    public List<SelectListItem> Locations { get; set; } = new();
    public List<SelectListItem> Users { get; set; } = new();
    public List<SelectListItem> Roles { get; set; } = new();
    public List<SelectListItem> Departments { get; set; } = new();

    public List<SelectListItem> SortOptions { get; set; } = new()
    {
        new SelectListItem { Value = "Name", Text = "Name" },
        new SelectListItem { Value = "CreatedDate", Text = "Created Date" },
        new SelectListItem { Value = "LastModified", Text = "Last Modified" },
        new SelectListItem { Value = "Price", Text = "Price" },
        new SelectListItem { Value = "Stock", Text = "Stock Level" }
    };

    public List<SelectListItem> SortDirections { get; set; } = new()
    {
        new SelectListItem { Value = "Ascending", Text = "Ascending" },
        new SelectListItem { Value = "Descending", Text = "Descending" }
    };

    // Saved filters functionality
    [Display(Name = "Save Filter")]
    public bool SaveFilter { get; set; }

    [Display(Name = "Filter Name")]
    [StringLength(50, ErrorMessage = "Filter name cannot exceed 50 characters")]
    public string? FilterName { get; set; }

    public List<SavedFilterViewModel> SavedFilters { get; set; } = new();

    public AdvancedFilterViewModel()
    {
        PageTitle = "Advanced Filter";
        PageSubtitle = "Create detailed search criteria";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Advanced Filter", null)
        };
    }
}

/// <summary>
/// Saved filter for reuse
/// </summary>
public class SavedFilterViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string FilterJson { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public DateTime? LastUsed { get; set; }
}
