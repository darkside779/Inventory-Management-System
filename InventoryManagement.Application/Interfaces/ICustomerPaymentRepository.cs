using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces;

/// <summary>
/// Repository interface for CustomerPayment entity
/// </summary>
public interface ICustomerPaymentRepository : IGenericRepository<CustomerPayment>
{
    /// <summary>
    /// Get payment by payment number
    /// </summary>
    /// <param name="paymentNumber">Payment number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payment if found</returns>
    Task<CustomerPayment?> GetByPaymentNumberAsync(string paymentNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get payments by customer ID
    /// </summary>
    /// <param name="customerId">Customer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Customer payments</returns>
    Task<IEnumerable<CustomerPayment>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get payments by invoice ID
    /// </summary>
    /// <param name="invoiceId">Invoice ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Invoice payments</returns>
    Task<IEnumerable<CustomerPayment>> GetByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get payments by date range
    /// </summary>
    /// <param name="fromDate">From date</param>
    /// <param name="toDate">To date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payments in date range</returns>
    Task<IEnumerable<CustomerPayment>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get payments by payment method
    /// </summary>
    /// <param name="paymentMethod">Payment method</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payments by method</returns>
    Task<IEnumerable<CustomerPayment>> GetByPaymentMethodAsync(string paymentMethod, CancellationToken cancellationToken = default);
}
