using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces;

/// <summary>
/// Repository interface for CustomerInvoice entity
/// </summary>
public interface ICustomerInvoiceRepository : IGenericRepository<CustomerInvoice>
{
    /// <summary>
    /// Get invoice by invoice number
    /// </summary>
    /// <param name="invoiceNumber">Invoice number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Invoice if found</returns>
    Task<CustomerInvoice?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get invoices by customer ID
    /// </summary>
    /// <param name="customerId">Customer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Customer invoices</returns>
    Task<IEnumerable<CustomerInvoice>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get customer's total invoice amount
    /// </summary>
    Task<decimal> GetCustomerTotalInvoicesAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search invoices with filtering, sorting and pagination
    /// </summary>
    Task<(IEnumerable<CustomerInvoice> Invoices, int TotalCount)> SearchInvoicesAsync(
        string? searchTerm = null,
        int? customerId = null,
        string? status = null,
        DateTime? invoiceDateFrom = null,
        DateTime? invoiceDateTo = null,
        DateTime? dueDateFrom = null,
        DateTime? dueDateTo = null,
        bool? isOverdue = null,
        int page = 1,
        int pageSize = 10,
        string sortBy = "InvoiceDate",
        string sortDirection = "desc",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get overdue invoices
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Overdue invoices</returns>
    Task<IEnumerable<CustomerInvoice>> GetOverdueInvoicesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get unpaid invoices
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Unpaid invoices</returns>
    Task<IEnumerable<CustomerInvoice>> GetUnpaidInvoicesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get invoice with items
    /// </summary>
    /// <param name="invoiceId">Invoice ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Invoice with items</returns>
    Task<CustomerInvoice?> GetWithItemsAsync(int invoiceId, CancellationToken cancellationToken = default);
}
