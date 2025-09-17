using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.ViewModels.User;

/// <summary>
/// ViewModel for editing existing users
/// </summary>
public class UserEditViewModel : BaseViewModel
{
    [Required]
    public string Id { get; set; } = string.Empty;

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

    [Required(ErrorMessage = "Role is required")]
    [Display(Name = "User Role")]
    public string Role { get; set; } = string.Empty;

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; }

    [Display(Name = "Email Confirmed")]
    public bool EmailConfirmed { get; set; }

    [Display(Name = "Account Locked")]
    public bool IsLockedOut { get; set; }

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

    // Read-only properties for display
    [Display(Name = "Created Date")]
    [DataType(DataType.DateTime)]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Last Login")]
    [DataType(DataType.DateTime)]
    public DateTime? LastLoginDate { get; set; }

    [Display(Name = "Failed Login Attempts")]
    public int AccessFailedCount { get; set; }

    [Display(Name = "Lockout End")]
    [DataType(DataType.DateTime)]
    public DateTime? LockoutEnd { get; set; }

    // Navigation properties for dropdowns
    public List<SelectListItem> Roles { get; set; } = new()
    {
        new SelectListItem { Value = "Employee", Text = "Employee" },
        new SelectListItem { Value = "Manager", Text = "Manager" },
        new SelectListItem { Value = "Admin", Text = "Administrator" }
    };

    public List<SelectListItem> Departments { get; set; } = new();

    // Password reset section
    [Display(Name = "Reset Password")]
    public bool ResetPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm New Password")]
    [Compare("NewPassword", ErrorMessage = "Password and confirmation password do not match")]
    public string? ConfirmNewPassword { get; set; }

    public UserEditViewModel()
    {
        PageTitle = "Edit User";
        PageSubtitle = "Update user account information";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("User Management", "/Account/UserManagement"),
            ("Edit User", null)
        };
    }
}
