using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.WebUI.ViewModels;

/// <summary>
/// View model for user management (Admin functionality)
/// </summary>
public class UserManagementViewModel
{
    [Display(Name = "User ID")]
    public string Id { get; set; } = string.Empty;

    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;

    [Display(Name = "Roles")]
    public IList<string> Roles { get; set; } = new List<string>();

    [Display(Name = "Is Locked Out")]
    public bool IsLockedOut { get; set; }
}
