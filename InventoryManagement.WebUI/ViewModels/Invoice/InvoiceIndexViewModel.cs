using InventoryManagement.Application.DTOs;

namespace InventoryManagement.WebUI.ViewModels.Invoice;

/// <summary>
/// ViewModel for invoice index page
/// </summary>
public class InvoiceIndexViewModel
{
    /// <summary>
    /// List of invoices
    /// </summary>
    public IEnumerable<CustomerInvoiceDto> Invoices { get; set; } = new List<CustomerInvoiceDto>();

    /// <summary>
    /// Current page number
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Total count of invoices
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Has previous page
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Has next page
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    // Filter properties
    /// <summary>
    /// Search term
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Customer ID filter
    /// </summary>
    public int? CustomerId { get; set; }

    /// <summary>
    /// Status filter
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Invoice date from
    /// </summary>
    public DateTime? InvoiceDateFrom { get; set; }

    /// <summary>
    /// Invoice date to
    /// </summary>
    public DateTime? InvoiceDateTo { get; set; }

    /// <summary>
    /// Due date from
    /// </summary>
    public DateTime? DueDateFrom { get; set; }

    /// <summary>
    /// Due date to
    /// </summary>
    public DateTime? DueDateTo { get; set; }

    /// <summary>
    /// Show only overdue invoices
    /// </summary>
    public bool? IsOverdue { get; set; }

    /// <summary>
    /// Sort by field
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction
    /// </summary>
    public string? SortDirection { get; set; }
}
