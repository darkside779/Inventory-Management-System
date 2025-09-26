using InventoryManagement.Application.DTOs;

namespace InventoryManagement.WebUI.ViewModels.Customer;

/// <summary>
/// View model for customer index page
/// </summary>
public class CustomerIndexViewModel
{
    /// <summary>
    /// List of customers
    /// </summary>
    public IEnumerable<CustomerDto> Customers { get; set; } = new List<CustomerDto>();

    /// <summary>
    /// Filter parameters
    /// </summary>
    public CustomerFilterViewModel Filter { get; set; } = new CustomerFilterViewModel();

    /// <summary>
    /// Total number of customers
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Check if there's a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Check if there's a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// Filter view model for customer search
/// </summary>
public class CustomerFilterViewModel
{
    /// <summary>
    /// Search term (name, code, email, phone)
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Customer type filter
    /// </summary>
    public string? CustomerType { get; set; }

    /// <summary>
    /// Active status filter
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Registration date from
    /// </summary>
    public DateTime? RegisteredFrom { get; set; }

    /// <summary>
    /// Registration date to
    /// </summary>
    public DateTime? RegisteredTo { get; set; }

    /// <summary>
    /// Sort field
    /// </summary>
    public string? SortBy { get; set; } = "FullName";

    /// <summary>
    /// Sort direction
    /// </summary>
    public string? SortDirection { get; set; } = "asc";
}
