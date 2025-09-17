namespace InventoryManagement.WebUI.ViewModels;

/// <summary>
/// View model for paginated data display
/// </summary>
/// <typeparam name="T">Type of items being paginated</typeparam>
public class PagedViewModel<T> : BaseViewModel
{
    /// <summary>
    /// Items for current page
    /// </summary>
    public IEnumerable<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Starting item number for current page
    /// </summary>
    public int StartItem => TotalCount == 0 ? 0 : (PageNumber - 1) * PageSize + 1;

    /// <summary>
    /// Ending item number for current page
    /// </summary>
    public int EndItem => Math.Min(PageNumber * PageSize, TotalCount);

    /// <summary>
    /// Search term used for filtering
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Sort field
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction (asc/desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";

    /// <summary>
    /// Additional filter parameters
    /// </summary>
    public Dictionary<string, string> Filters { get; set; } = new();
}
