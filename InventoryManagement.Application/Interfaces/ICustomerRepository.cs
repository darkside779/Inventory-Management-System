using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces;

/// <summary>
/// Repository interface for Customer entity
/// </summary>
public interface ICustomerRepository : IGenericRepository<Customer>
{
    /// <summary>
    /// Get customer by customer code
    /// </summary>
    /// <param name="customerCode">Customer code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Customer if found</returns>
    Task<Customer?> GetByCustomerCodeAsync(string customerCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get customers by email
    /// </summary>
    /// <param name="email">Email address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Customers with matching email</returns>
    Task<IEnumerable<Customer>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get customers with outstanding balance
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Customers with positive balance</returns>
    Task<IEnumerable<Customer>> GetCustomersWithBalanceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get customers over credit limit
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Customers exceeding credit limit</returns>
    Task<IEnumerable<Customer>> GetCustomersOverCreditLimitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Search customers by multiple criteria
    /// </summary>
    /// <param name="searchTerm">Search term (name, code, email, phone)</param>
    /// <param name="customerType">Customer type filter</param>
    /// <param name="isActive">Active status filter</param>
    /// <param name="registeredFrom">Registration date from</param>
    /// <param name="registeredTo">Registration date to</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="sortBy">Sort field</param>
    /// <param name="sortDirection">Sort direction</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Filtered customers</returns>
    Task<(IEnumerable<Customer> Customers, int TotalCount)> SearchCustomersAsync(
        string? searchTerm = null,
        string? customerType = null,
        bool? isActive = null,
        DateTime? registeredFrom = null,
        DateTime? registeredTo = null,
        int page = 1,
        int pageSize = 10,
        string sortBy = "FullName",
        string sortDirection = "asc",
        CancellationToken cancellationToken = default);
}
