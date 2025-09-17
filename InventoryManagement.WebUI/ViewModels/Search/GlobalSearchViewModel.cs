using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.WebUI.ViewModels.Search;

/// <summary>
/// ViewModel for global search functionality across all entities
/// </summary>
public class GlobalSearchViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Search term is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Search term must be between 2 and 100 characters")]
    [Display(Name = "Search")]
    public string SearchTerm { get; set; } = string.Empty;

    [Display(Name = "Search Type")]
    public string SearchType { get; set; } = "All";

    [Display(Name = "Category Filter")]
    public int? CategoryId { get; set; }

    [Display(Name = "Include Inactive")]
    public bool IncludeInactive { get; set; } = false;

    // Search Results
    public GlobalSearchResultsViewModel Results { get; set; } = new();

    public GlobalSearchViewModel()
    {
        PageTitle = "Global Search";
        PageSubtitle = "Search across products, categories, and transactions";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Search Results", null)
        };
    }
}

/// <summary>
/// Container for all search results
/// </summary>
public class GlobalSearchResultsViewModel
{
    public List<SearchResultItemViewModel> Products { get; set; } = new();
    public List<SearchResultItemViewModel> Categories { get; set; } = new();
    public List<SearchResultItemViewModel> Transactions { get; set; } = new();
    public List<SearchResultItemViewModel> Users { get; set; } = new();

    public int TotalResults => Products.Count + Categories.Count + Transactions.Count + Users.Count;
    public bool HasResults => TotalResults > 0;
}

/// <summary>
/// Individual search result item
/// </summary>
public class SearchResultItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Icon { get; set; } = "fas fa-search";
    public string BadgeText { get; set; } = string.Empty;
    public string BadgeClass { get; set; } = "badge bg-primary";
    public DateTime? LastModified { get; set; }
    public double RelevanceScore { get; set; } = 1.0;

    public string HighlightedTitle { get; set; } = string.Empty;
    public string HighlightedDescription { get; set; } = string.Empty;
}
