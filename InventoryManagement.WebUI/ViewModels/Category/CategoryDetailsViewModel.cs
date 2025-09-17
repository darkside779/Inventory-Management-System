using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.WebUI.ViewModels.Category;

/// <summary>
/// ViewModel for displaying category details
/// </summary>
public class CategoryDetailsViewModel : BaseViewModel
{
    public int Id { get; set; }

    [Display(Name = "Category Name")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Status")]
    public string Status => IsActive ? "Active" : "Inactive";

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; }

    [Display(Name = "Color Code")]
    public string? ColorCode { get; set; }

    [Display(Name = "Icon")]
    public string? Icon { get; set; }

    [Display(Name = "Sort Order")]
    public int SortOrder { get; set; }

    [Display(Name = "Parent Category")]
    public string? ParentCategoryName { get; set; }

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

    // Statistics
    [Display(Name = "Total Products")]
    public int ProductCount { get; set; }

    [Display(Name = "Active Products")]
    public int ActiveProductCount { get; set; }

    [Display(Name = "Subcategories")]
    public int SubcategoryCount { get; set; }

    [Display(Name = "Total Category Value")]
    [DataType(DataType.Currency)]
    public decimal TotalCategoryValue { get; set; }

    [Display(Name = "Low Stock Products")]
    public int LowStockProductCount { get; set; }

    // Subcategories list
    public List<SubcategoryViewModel> Subcategories { get; set; } = new();

    // Recent products in this category
    public List<CategoryProductViewModel> RecentProducts { get; set; } = new();

    public CategoryDetailsViewModel()
    {
        PageTitle = "Category Details";
        PageSubtitle = "View category information and statistics";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Categories", "/Category"),
            ("Category Details", null)
        };
    }
}

/// <summary>
/// Subcategory view model for category details
/// </summary>
public class SubcategoryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int ProductCount { get; set; }
    public string? ColorCode { get; set; }
    public string? Icon { get; set; }
}

/// <summary>
/// Product view model for category details
/// </summary>
public class CategoryProductViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int CurrentStock { get; set; }
    public bool IsLowStock { get; set; }
    public DateTime LastModified { get; set; }
}
