using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.WebUI.ViewModels.User;

/// <summary>
/// ViewModel for displaying user details
/// </summary>
public class UserDetailsViewModel : BaseViewModel
{
    public string Id { get; set; } = string.Empty;

    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;

    [Display(Name = "Role")]
    public string Role { get; set; } = string.Empty;

    [Display(Name = "Status")]
    public string Status => IsActive ? "Active" : "Inactive";

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; }

    [Display(Name = "Email Confirmed")]
    public bool EmailConfirmed { get; set; }

    [Display(Name = "Account Status")]
    public string AccountStatus => IsLockedOut ? "Locked Out" : 
                                  !EmailConfirmed ? "Email Not Confirmed" : 
                                  IsActive ? "Active" : "Inactive";

    [Display(Name = "Is Locked Out")]
    public bool IsLockedOut { get; set; }

    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Department")]
    public string? Department { get; set; }

    [Display(Name = "Job Title")]
    public string? JobTitle { get; set; }

    [Display(Name = "Notes")]
    public string? Notes { get; set; }

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

    [Display(Name = "Two Factor Enabled")]
    public bool TwoFactorEnabled { get; set; }

    // Activity Statistics
    [Display(Name = "Total Logins")]
    public int TotalLogins { get; set; }

    [Display(Name = "Last 30 Days Activity")]
    public int ActivityLast30Days { get; set; }

    [Display(Name = "Products Created")]
    public int ProductsCreated { get; set; }

    [Display(Name = "Inventory Transactions")]
    public int InventoryTransactions { get; set; }

    [Display(Name = "Categories Created")]
    public int CategoriesCreated { get; set; }

    // Recent activities performed by this user
    public List<UserActivityViewModel> RecentActivities { get; set; } = new();

    // Security information
    [Display(Name = "Security Score")]
    public string SecurityScore => CalculateSecurityScore();

    private string CalculateSecurityScore()
    {
        var score = 0;
        if (EmailConfirmed) score += 25;
        if (TwoFactorEnabled) score += 25;
        if (!string.IsNullOrEmpty(PhoneNumber)) score += 15;
        if (AccessFailedCount == 0) score += 20;
        if (LastLoginDate.HasValue && LastLoginDate.Value > DateTime.Now.AddDays(-30)) score += 15;

        return score switch
        {
            >= 80 => "Excellent",
            >= 60 => "Good",
            >= 40 => "Fair",
            _ => "Poor"
        };
    }

    public UserDetailsViewModel()
    {
        PageTitle = "User Details";
        PageSubtitle = "View user account information and activity";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("User Management", "/Account/UserManagement"),
            ("User Details", null)
        };
    }
}

/// <summary>
/// User activity for user details view
/// </summary>
public class UserActivityViewModel
{
    public DateTime ActivityDate { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }

    public string TimeAgo
    {
        get
        {
            var timeSpan = DateTime.Now - ActivityDate;
            if (timeSpan.TotalMinutes < 1) return "Just now";
            if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays} days ago";
            return ActivityDate.ToString("MMM dd, yyyy");
        }
    }
}
