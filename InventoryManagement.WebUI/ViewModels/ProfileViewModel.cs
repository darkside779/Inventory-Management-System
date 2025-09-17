using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.WebUI.ViewModels;

/// <summary>
/// View model for user profile display
/// </summary>
public class ProfileViewModel
{
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;

    [Display(Name = "Roles")]
    public IList<string> Roles { get; set; } = new List<string>();
}
