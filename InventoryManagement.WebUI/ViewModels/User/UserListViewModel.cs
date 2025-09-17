using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.ViewModels.User;

/// <summary>
/// ViewModel for user list with search and filtering
/// </summary>
public class UserListViewModel : PagedViewModel<UserItemViewModel>
{
    // Search and filter properties
    [Display(Name = "Search")]
    public new string? SearchTerm { get; set; }

    [Display(Name = "Role")]
    public string? Role { get; set; }

    [Display(Name = "Status")]
    public bool? IsActive { get; set; }

    [Display(Name = "Department")]
    public string? Department { get; set; }

    [Display(Name = "Account Status")]
    public string? AccountStatus { get; set; }

    [Display(Name = "Last Login From")]
    [DataType(DataType.Date)]
    public DateTime? LastLoginFrom { get; set; }

    [Display(Name = "Last Login To")]
    [DataType(DataType.Date)]
    public DateTime? LastLoginTo { get; set; }

    // Filter options for dropdowns
    public List<SelectListItem> Roles { get; set; } = new()
    {
        new SelectListItem { Value = "", Text = "All Roles" },
        new SelectListItem { Value = "Admin", Text = "Administrator" },
        new SelectListItem { Value = "Manager", Text = "Manager" },
        new SelectListItem { Value = "Employee", Text = "Employee" }
    };

    public List<SelectListItem> Departments { get; set; } = new();

    public List<SelectListItem> AccountStatuses { get; set; } = new()
    {
        new SelectListItem { Value = "", Text = "All Statuses" },
        new SelectListItem { Value = "Active", Text = "Active" },
        new SelectListItem { Value = "Inactive", Text = "Inactive" },
        new SelectListItem { Value = "LockedOut", Text = "Locked Out" },
        new SelectListItem { Value = "EmailNotConfirmed", Text = "Email Not Confirmed" }
    };

    // Summary information
    [Display(Name = "Total Users")]
    public int TotalUsers { get; set; }

    [Display(Name = "Active Users")]
    public int ActiveUsers { get; set; }

    [Display(Name = "Locked Out Users")]
    public int LockedOutUsers { get; set; }

    [Display(Name = "Administrators")]
    public int AdminUsers { get; set; }

    [Display(Name = "Managers")]
    public int ManagerUsers { get; set; }

    [Display(Name = "Employees")]
    public int EmployeeUsers { get; set; }

    public UserListViewModel()
    {
        PageTitle = "User Management";
        PageSubtitle = "Manage system users and access control";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("User Management", null)
        };
    }
}

/// <summary>
/// Individual user item for list display
/// </summary>
public class UserItemViewModel
{
    public string Id { get; set; } = string.Empty;

    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;

    [Display(Name = "Role")]
    public string Role { get; set; } = string.Empty;

    [Display(Name = "Department")]
    public string? Department { get; set; }

    [Display(Name = "Job Title")]
    public string? JobTitle { get; set; }

    [Display(Name = "Status")]
    public string Status => IsActive ? "Active" : "Inactive";

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; }

    [Display(Name = "Email Confirmed")]
    public bool EmailConfirmed { get; set; }

    [Display(Name = "Is Locked Out")]
    public bool IsLockedOut { get; set; }

    [Display(Name = "Created Date")]
    [DataType(DataType.DateTime)]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Last Login")]
    [DataType(DataType.DateTime)]
    public DateTime? LastLoginDate { get; set; }

    [Display(Name = "Failed Login Attempts")]
    public int AccessFailedCount { get; set; }

    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    // Computed properties for display
    [Display(Name = "Account Status")]
    public string AccountStatus => IsLockedOut ? "Locked Out" : 
                                  !EmailConfirmed ? "Email Not Confirmed" : 
                                  IsActive ? "Active" : "Inactive";

    [Display(Name = "Last Activity")]
    public string LastActivity => LastLoginDate.HasValue ? 
        $"{(DateTime.Now - LastLoginDate.Value).Days} days ago" : "Never";

    public bool HasRecentActivity => LastLoginDate.HasValue && 
        LastLoginDate.Value > DateTime.Now.AddDays(-30);

    // CSS classes for styling
    public string StatusCssClass => IsActive ? "badge bg-success" : "badge bg-secondary";
    
    public string AccountStatusCssClass => AccountStatus switch
    {
        "Active" => "badge bg-success",
        "Locked Out" => "badge bg-danger",
        "Email Not Confirmed" => "badge bg-warning",
        "Inactive" => "badge bg-secondary",
        _ => "badge bg-secondary"
    };

    public string RoleCssClass => Role switch
    {
        "Admin" => "badge bg-danger",
        "Manager" => "badge bg-warning",
        "Employee" => "badge bg-info",
        _ => "badge bg-secondary"
    };

    public string ActivityIndicator => HasRecentActivity ? "text-success" : "text-muted";
}
