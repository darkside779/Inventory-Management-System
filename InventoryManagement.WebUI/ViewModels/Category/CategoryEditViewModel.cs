using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.WebUI.ViewModels.Category;

/// <summary>
/// ViewModel for editing existing categories
/// </summary>
public class CategoryEditViewModel : BaseViewModel
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
    [Display(Name = "Category Name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; }

    [StringLength(10, ErrorMessage = "Color code cannot exceed 10 characters")]
    [Display(Name = "Color Code")]
    [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Please enter a valid hex color code (e.g., #FF0000)")]
    public string? ColorCode { get; set; }

    [StringLength(50, ErrorMessage = "Icon cannot exceed 50 characters")]
    [Display(Name = "Icon Class")]
    public string? Icon { get; set; }

    [Range(0, 999, ErrorMessage = "Sort order must be between 0 and 999")]
    [Display(Name = "Sort Order")]
    public int SortOrder { get; set; }

    [Display(Name = "Parent Category")]
    public int? ParentCategoryId { get; set; }

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

    [Display(Name = "Product Count")]
    public int ProductCount { get; set; }

    [Display(Name = "Subcategory Count")]
    public int SubcategoryCount { get; set; }

    // Navigation properties
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ParentCategories { get; set; } = new();

    public CategoryEditViewModel()
    {
        PageTitle = "Edit Category";
        PageSubtitle = "Update category information";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Categories", "/Category"),
            ("Edit Category", null)
        };
    }
}
