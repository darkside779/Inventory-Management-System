using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.ViewModels.Category;

/// <summary>
/// ViewModel for category list with search and filtering
/// </summary>
public class CategoryListViewModel : PagedViewModel<CategoryItemViewModel>
{
    // Search and filter properties
    [Display(Name = "Search")]
    public new string? SearchTerm { get; set; }

    [Display(Name = "Status")]
    public bool? IsActive { get; set; }

    [Display(Name = "Parent Category")]
    public int? ParentCategoryId { get; set; }

    [Display(Name = "Has Products")]
    public bool? HasProducts { get; set; }

    // Filter options for dropdowns
    public List<SelectListItem> ParentCategories { get; set; } = new();

    // Summary information
    [Display(Name = "Total Categories")]
    public int TotalCategories { get; set; }

    [Display(Name = "Active Categories")]
    public int ActiveCategories { get; set; }

    [Display(Name = "Categories with Products")]
    public int CategoriesWithProducts { get; set; }

    [Display(Name = "Total Products Across All Categories")]
    public int TotalProducts { get; set; }

    public CategoryListViewModel()
    {
        PageTitle = "Category Management";
        PageSubtitle = "Manage product categories and organization";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Categories", null)
        };
    }
}

/// <summary>
/// Individual category item for list display
/// </summary>
public class CategoryItemViewModel
{
    public int Id { get; set; }

    [Display(Name = "Category Name")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Parent Category")]
    public string? ParentCategoryName { get; set; }

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

    [Display(Name = "Product Count")]
    public int ProductCount { get; set; }

    [Display(Name = "Subcategory Count")]
    public int SubcategoryCount { get; set; }

    [Display(Name = "Last Modified")]
    [DataType(DataType.DateTime)]
    public DateTime? LastModified { get; set; }

    // Computed properties for display
    [Display(Name = "Has Products")]
    public bool HasProducts => ProductCount > 0;

    [Display(Name = "Has Subcategories")]
    public bool HasSubcategories => SubcategoryCount > 0;

    [Display(Name = "Category Level")]
    public string CategoryLevel => ParentCategoryName == null ? "Main Category" : "Subcategory";

    // CSS classes for styling
    public string StatusCssClass => IsActive ? "badge bg-success" : "badge bg-secondary";
    
    public string ColorIndicator => !string.IsNullOrEmpty(ColorCode) ? ColorCode : "#6c757d";

    public string IconClass => !string.IsNullOrEmpty(Icon) ? Icon : "fas fa-folder";
}
