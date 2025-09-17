using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.WebUI.ViewModels.Users;

/// <summary>
/// ViewModel for user list/index page
/// </summary>
public class UserIndexViewModel
{
    /// <summary>
    /// List of users
    /// </summary>
    public PagedResult<UserDto> Users { get; set; } = new();

    /// <summary>
    /// Filter criteria
    /// </summary>
    public UserFilterViewModel Filter { get; set; } = new();

    /// <summary>
    /// Available roles for filtering
    /// </summary>
    public SelectList RoleOptions { get; set; } = new(new List<SelectListItem>());

    /// <summary>
    /// Current sort field
    /// </summary>
    public string CurrentSort { get; set; } = string.Empty;

    /// <summary>
    /// Current sort direction
    /// </summary>
    public string SortDirection { get; set; } = "asc";
}

/// <summary>
/// ViewModel for filtering users
/// </summary>
public class UserFilterViewModel
{
    /// <summary>
    /// Search term for name, username, or email
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter by role
    /// </summary>
    public UserRole? Role { get; set; }

    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Date range start for user creation
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? CreatedFrom { get; set; }

    /// <summary>
    /// Date range end for user creation
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? CreatedTo { get; set; }

    /// <summary>
    /// Show only users who have logged in recently
    /// </summary>
    public bool? HasRecentLogin { get; set; }

    /// <summary>
    /// Page number
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 25;

    /// <summary>
    /// Sort field
    /// </summary>
    public string SortBy { get; set; } = "FullName";

    /// <summary>
    /// Sort direction
    /// </summary>
    public string SortDirection { get; set; } = "asc";
}

/// <summary>
/// ViewModel for creating a new user
/// </summary>
public class CreateUserViewModel
{
    /// <summary>
    /// Username of the user
    /// </summary>
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
    [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "Username can only contain letters, numbers, dots, hyphens, and underscores")]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Email address of the user
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password for the user
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Confirm password
    /// </summary>
    [Required(ErrorMessage = "Please confirm the password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the user
    /// </summary>
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Phone number
    /// </summary>
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Role of the user
    /// </summary>
    [Required(ErrorMessage = "Please select a role")]
    [Display(Name = "Role")]
    public UserRole Role { get; set; }

    /// <summary>
    /// Whether the user is active
    /// </summary>
    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Available roles for selection
    /// </summary>
    public SelectList RoleOptions { get; set; } = new(new List<SelectListItem>());

    /// <summary>
    /// Whether to send welcome email
    /// </summary>
    [Display(Name = "Send Welcome Email")]
    public bool SendWelcomeEmail { get; set; } = true;
}

/// <summary>
/// ViewModel for editing an existing user
/// </summary>
public class EditUserViewModel
{
    /// <summary>
    /// User ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Username of the user
    /// </summary>
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
    [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "Username can only contain letters, numbers, dots, hyphens, and underscores")]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Email address of the user
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the user
    /// </summary>
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Phone number
    /// </summary>
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Role of the user
    /// </summary>
    [Required(ErrorMessage = "Please select a role")]
    [Display(Name = "Role")]
    public UserRole Role { get; set; }

    /// <summary>
    /// Whether the user is active
    /// </summary>
    [Display(Name = "Active")]
    public bool IsActive { get; set; }

    /// <summary>
    /// Available roles for selection
    /// </summary>
    public SelectList RoleOptions { get; set; } = new(new List<SelectListItem>());

    /// <summary>
    /// Created date (read-only)
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last login date (read-only)
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Current user can edit this user
    /// </summary>
    public bool CanEdit { get; set; } = true;

    /// <summary>
    /// Current user can delete this user
    /// </summary>
    public bool CanDelete { get; set; } = true;
}

/// <summary>
/// ViewModel for user details page
/// </summary>
public class UserDetailsViewModel
{
    /// <summary>
    /// User information
    /// </summary>
    public UserDto User { get; set; } = new();

    /// <summary>
    /// User statistics
    /// </summary>
    public UserStatsViewModel Stats { get; set; } = new();

    /// <summary>
    /// Recent activities
    /// </summary>
    public List<UserActivityViewModel> RecentActivities { get; set; } = new();

    /// <summary>
    /// Current user can edit this user
    /// </summary>
    public bool CanEdit { get; set; }

    /// <summary>
    /// Current user can delete this user
    /// </summary>
    public bool CanDelete { get; set; }

    /// <summary>
    /// Current user can reset password
    /// </summary>
    public bool CanResetPassword { get; set; }
}

/// <summary>
/// ViewModel for user statistics
/// </summary>
public class UserStatsViewModel
{
    /// <summary>
    /// Total transactions created by user
    /// </summary>
    public int TotalTransactions { get; set; }

    /// <summary>
    /// Transactions in last 30 days
    /// </summary>
    public int TransactionsLast30Days { get; set; }

    /// <summary>
    /// Total value of transactions
    /// </summary>
    public decimal TotalTransactionValue { get; set; }

    /// <summary>
    /// Days since last login
    /// </summary>
    public int? DaysSinceLastLogin { get; set; }

    /// <summary>
    /// Account age in days
    /// </summary>
    public int AccountAgeInDays { get; set; }

    /// <summary>
    /// Login frequency (logins per month)
    /// </summary>
    public double LoginFrequency { get; set; }
}

/// <summary>
/// ViewModel for user activity log
/// </summary>
public class UserActivityViewModel
{
    /// <summary>
    /// Activity timestamp
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Activity type
    /// </summary>
    public string ActivityType { get; set; } = string.Empty;

    /// <summary>
    /// Activity description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// IP address
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent
    /// </summary>
    public string? UserAgent { get; set; }
}

/// <summary>
/// ViewModel for changing user password
/// </summary>
public class ChangePasswordViewModel
{
    /// <summary>
    /// User ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Username (read-only)
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Full name (read-only)
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Current password (if changing own password)
    /// </summary>
    [DataType(DataType.Password)]
    [Display(Name = "Current Password")]
    public string? CurrentPassword { get; set; }

    /// <summary>
    /// New password
    /// </summary>
    [Required(ErrorMessage = "New password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Confirm new password
    /// </summary>
    [Required(ErrorMessage = "Please confirm the new password")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match")]
    [Display(Name = "Confirm New Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Whether this is a password reset by admin
    /// </summary>
    public bool IsAdminReset { get; set; }

    /// <summary>
    /// Whether to send password change notification
    /// </summary>
    [Display(Name = "Send Password Change Notification")]
    public bool SendNotification { get; set; } = true;
}

/// <summary>
/// ViewModel for user profile management
/// </summary>
public class UserProfileViewModel
{
    /// <summary>
    /// User ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Username (read-only)
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Email address
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Full name
    /// </summary>
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Phone number
    /// </summary>
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Role (read-only)
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Created date (read-only)
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last login date (read-only)
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
}

/// <summary>
/// Generic paged result wrapper
/// </summary>
/// <typeparam name="T">Type of items in the result</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Items in current page
    /// </summary>
    public IEnumerable<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Total number of items
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => Page < TotalPages;
}
