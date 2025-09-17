using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.ViewModels.User;

/// <summary>
/// ViewModel for creating new users
/// </summary>
public class UserCreateViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    [RegularExpression(@"^[a-zA-Z0-9_.-]+$", ErrorMessage = "Username can only contain letters, numbers, dots, hyphens, and underscores")]
    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password confirmation is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required")]
    [Display(Name = "User Role")]
    public string Role { get; set; } = string.Empty;

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [StringLength(200, ErrorMessage = "Department cannot exceed 200 characters")]
    [Display(Name = "Department")]
    public string? Department { get; set; }

    [StringLength(100, ErrorMessage = "Job title cannot exceed 100 characters")]
    [Display(Name = "Job Title")]
    public string? JobTitle { get; set; }

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    // Navigation properties for dropdowns
    public List<SelectListItem> Roles { get; set; } = new()
    {
        new SelectListItem { Value = "Employee", Text = "Employee" },
        new SelectListItem { Value = "Manager", Text = "Manager" },
        new SelectListItem { Value = "Admin", Text = "Administrator" }
    };

    public List<SelectListItem> Departments { get; set; } = new();

    public UserCreateViewModel()
    {
        PageTitle = "Add New User";
        PageSubtitle = "Create a new user account";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("User Management", "/Account/UserManagement"),
            ("Add User", null)
        };
    }
}
