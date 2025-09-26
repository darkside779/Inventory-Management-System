using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for CustomerPayment entity
/// </summary>
public class CustomerPaymentRepository : GenericRepository<CustomerPayment>, ICustomerPaymentRepository
{
    public CustomerPaymentRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get payment by payment number
    /// </summary>
    public async Task<CustomerPayment?> GetByPaymentNumberAsync(string paymentNumber, CancellationToken cancellationToken = default)
    {
        return await _context.CustomerPayments
            .Include(p => p.Customer)
            .Include(p => p.Invoice)
            .Include(p => p.RecordedByUser)
            .FirstOrDefaultAsync(p => p.PaymentNumber == paymentNumber, cancellationToken);
    }

    /// <summary>
    /// Get payments by customer ID
    /// </summary>
    public async Task<IEnumerable<CustomerPayment>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await _context.CustomerPayments
            .Include(p => p.Invoice)
            .Include(p => p.RecordedByUser)
            .Where(p => p.CustomerId == customerId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get payments by invoice ID
    /// </summary>
    public async Task<IEnumerable<CustomerPayment>> GetByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken = default)
    {
        return await _context.CustomerPayments
            .Include(p => p.Customer)
            .Include(p => p.RecordedByUser)
            .Where(p => p.InvoiceId == invoiceId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get payments by date range
    /// </summary>
    public async Task<IEnumerable<CustomerPayment>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.CustomerPayments
            .Include(p => p.Customer)
            .Include(p => p.Invoice)
            .Include(p => p.RecordedByUser)
            .Where(p => p.PaymentDate >= fromDate && p.PaymentDate <= toDate)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get payments by payment method
    /// </summary>
    public async Task<IEnumerable<CustomerPayment>> GetByPaymentMethodAsync(string paymentMethod, CancellationToken cancellationToken = default)
    {
        return await _context.CustomerPayments
            .Include(p => p.Customer)
            .Include(p => p.Invoice)
            .Include(p => p.RecordedByUser)
            .Where(p => p.PaymentMethod == paymentMethod)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }
}
