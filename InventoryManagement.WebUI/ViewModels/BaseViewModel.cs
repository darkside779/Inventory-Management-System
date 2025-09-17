namespace InventoryManagement.WebUI.ViewModels;

/// <summary>
/// Base view model with common properties
/// </summary>
public abstract class BaseViewModel
{
    /// <summary>
    /// Page title
    /// </summary>
    public string? PageTitle { get; set; }

    /// <summary>
    /// Page subtitle
    /// </summary>
    public string? PageSubtitle { get; set; }

    /// <summary>
    /// Breadcrumb items for navigation
    /// </summary>
    public List<(string text, string? url)> BreadcrumbItems { get; set; } = new();

    /// <summary>
    /// Loading state indicator
    /// </summary>
    public bool IsLoading { get; set; }

    /// <summary>
    /// Error message to display
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Success message to display
    /// </summary>
    public string? SuccessMessage { get; set; }
}
