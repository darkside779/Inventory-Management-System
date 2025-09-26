using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for CustomerInvoice entity
/// </summary>
public class CustomerInvoiceRepository : GenericRepository<CustomerInvoice>, ICustomerInvoiceRepository
{
    public CustomerInvoiceRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get invoice by invoice number
    /// </summary>
    public async Task<CustomerInvoice?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default)
    {
        return await _context.CustomerInvoices
            .Include(i => i.Customer)
            .Include(i => i.Items)
                .ThenInclude(item => item.Product)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber, cancellationToken);
    }

    /// <summary>
    /// Get invoices by customer ID
    /// </summary>
    public async Task<IEnumerable<CustomerInvoice>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await _context.CustomerInvoices
            .Include(i => i.Items)
                .ThenInclude(item => item.Product)
            .Include(i => i.Payments)
            .Where(i => i.CustomerId == customerId)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get overdue invoices
    /// </summary>
    public async Task<IEnumerable<CustomerInvoice>> GetOverdueInvoicesAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        return await _context.CustomerInvoices
            .Include(i => i.Customer)
            .Where(i => i.DueDate < today && i.PaidAmount < i.TotalAmount && i.Status != "Cancelled")
            .OrderBy(i => i.DueDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get unpaid invoices
    /// </summary>
    public async Task<IEnumerable<CustomerInvoice>> GetUnpaidInvoicesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CustomerInvoices
            .Include(i => i.Customer)
            .Where(i => i.PaidAmount < i.TotalAmount && i.Status != "Cancelled")
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get invoice with items
    /// </summary>
    public async Task<CustomerInvoice?> GetWithItemsAsync(int invoiceId, CancellationToken cancellationToken = default)
    {
        return await _context.CustomerInvoices
            .Include(i => i.Customer)
            .Include(i => i.Items)
                .ThenInclude(item => item.Product)
            .Include(i => i.Payments)
            .Include(i => i.CreatedByUser)
            .FirstOrDefaultAsync(i => i.Id == invoiceId, cancellationToken);
    }

    /// <summary>
    /// Get customer's total invoice amount
    /// </summary>
    public async Task<decimal> GetCustomerTotalInvoicesAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await _context.CustomerInvoices
            .Where(i => i.CustomerId == customerId && i.IsActive)
            .SumAsync(i => i.TotalAmount, cancellationToken);
    }

    public async Task<(IEnumerable<CustomerInvoice> Invoices, int TotalCount)> SearchInvoicesAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = _context.CustomerInvoices
            .Include(i => i.Customer)
            .Include(i => i.Items)
            .ThenInclude(item => item.Product)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(i =>
                i.InvoiceNumber.Contains(searchTerm) ||
                i.Customer.FullName.Contains(searchTerm) ||
                (i.Customer.CompanyName != null && i.Customer.CompanyName.Contains(searchTerm)) ||
                (i.Notes != null && i.Notes.Contains(searchTerm)));
        }

        if (customerId.HasValue)
        {
            query = query.Where(i => i.CustomerId == customerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(i => i.Status == status);
        }

        if (invoiceDateFrom.HasValue)
        {
            query = query.Where(i => i.InvoiceDate >= invoiceDateFrom.Value);
        }

        if (invoiceDateTo.HasValue)
        {
            query = query.Where(i => i.InvoiceDate <= invoiceDateTo.Value);
        }

        if (dueDateFrom.HasValue)
        {
            query = query.Where(i => i.DueDate >= dueDateFrom.Value);
        }

        if (dueDateTo.HasValue)
        {
            query = query.Where(i => i.DueDate <= dueDateTo.Value);
        }

        if (isOverdue.HasValue && isOverdue.Value)
        {
            var today = DateTime.UtcNow.Date;
            query = query.Where(i => i.DueDate < today && i.TotalAmount > i.PaidAmount);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting (simplified)
        if (sortDirection?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true)
        {
            query = query.OrderByDescending(i => i.InvoiceDate);
        }
        else
        {
            query = query.OrderBy(i => i.InvoiceDate);
        }

        // Apply pagination
        var invoices = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (invoices, totalCount);
    }
}
