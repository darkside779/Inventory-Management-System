using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.WebUI.ViewModels.Category;

/// <summary>
/// Base category ViewModel
/// </summary>
public class CategoryViewModel : BaseViewModel
{
    public int Id { get; set; }

    [Display(Name = "Category Name")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; }

    [Display(Name = "Created Date")]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Updated Date")]
    public DateTime UpdatedAt { get; set; }

    [Display(Name = "Product Count")]
    public int ProductCount { get; set; }
}

/// <summary>
/// ViewModel for category index/listing page
/// </summary>
public class CategoryIndexViewModel : BaseViewModel
{
    public List<CategoryViewModel> Categories { get; set; } = new();
    public bool CanCreate { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }

    public CategoryIndexViewModel()
    {
        PageTitle = "Categories";
        PageSubtitle = "Manage Product Categories";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Categories", null)
        };
    }
}

/// <summary>
/// ViewModel for creating a new category
/// </summary>
public class CreateCategoryViewModel : BaseViewModel
{
    [Required]
    [Display(Name = "Category Name")]
    [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Description")]
    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public CreateCategoryViewModel()
    {
        PageTitle = "Create Category";
        PageSubtitle = "Add New Category";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Categories", "/Category"),
            ("Create", null)
        };
    }
}

/// <summary>
/// ViewModel for editing an existing category
/// </summary>
public class EditCategoryViewModel : BaseViewModel
{
    [Required]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Category Name")]
    [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Description")]
    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; }

    public EditCategoryViewModel()
    {
        PageTitle = "Edit Category";
        PageSubtitle = "Update Category Information";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Categories", "/Category"),
            ("Edit", null)
        };
    }
}


/// <summary>
/// ViewModel for category deletion confirmation
/// </summary>
public class DeleteCategoryViewModel : BaseViewModel
{
    public int Id { get; set; }

    [Display(Name = "Category Name")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Product Count")]
    public int ProductCount { get; set; }

    [Display(Name = "Created Date")]
    public DateTime CreatedAt { get; set; }

    public DeleteCategoryViewModel()
    {
        PageTitle = "Delete Category";
        PageSubtitle = "Confirm Category Deletion";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Categories", "/Category"),
            ("Delete", null)
        };
    }
}
