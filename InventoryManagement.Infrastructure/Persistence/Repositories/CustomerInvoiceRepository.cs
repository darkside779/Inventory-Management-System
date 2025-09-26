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
}
