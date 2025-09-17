using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.WebUI.ViewModels.Category;

/// <summary>
/// ViewModel for creating new categories
/// </summary>
public class CategoryCreateViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
    [Display(Name = "Category Name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    [StringLength(10, ErrorMessage = "Color code cannot exceed 10 characters")]
    [Display(Name = "Color Code")]
    [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Please enter a valid hex color code (e.g., #FF0000)")]
    public string? ColorCode { get; set; } = "#007bff";

    [StringLength(50, ErrorMessage = "Icon cannot exceed 50 characters")]
    [Display(Name = "Icon Class")]
    public string? Icon { get; set; }

    [Range(0, 999, ErrorMessage = "Sort order must be between 0 and 999")]
    [Display(Name = "Sort Order")]
    public int SortOrder { get; set; } = 0;

    [Display(Name = "Parent Category")]
    public int? ParentCategoryId { get; set; }

    // Navigation properties
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ParentCategories { get; set; } = new();

    public CategoryCreateViewModel()
    {
        PageTitle = "Add New Category";
        PageSubtitle = "Create a new product category";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Categories", "/Category"),
            ("Add Category", null)
        };
    }
}
