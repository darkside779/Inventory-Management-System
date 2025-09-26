using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Customer entity
/// </summary>
public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get customer by customer code
    /// </summary>
    public async Task<Customer?> GetByCustomerCodeAsync(string customerCode, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.CustomerCode == customerCode, cancellationToken);
    }

    /// <summary>
    /// Get customers by email
    /// </summary>
    public async Task<IEnumerable<Customer>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Where(c => c.Email == email)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get customers with outstanding balance
    /// </summary>
    public async Task<IEnumerable<Customer>> GetCustomersWithBalanceAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Where(c => c.Balance > 0 && c.IsActive)
            .OrderByDescending(c => c.Balance)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get customers over credit limit
    /// </summary>
    public async Task<IEnumerable<Customer>> GetCustomersOverCreditLimitAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Where(c => c.Balance > c.CreditLimit && c.IsActive)
            .OrderByDescending(c => c.Balance - c.CreditLimit)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Search customers by multiple criteria
    /// </summary>
    public async Task<(IEnumerable<Customer> Customers, int TotalCount)> SearchCustomersAsync(
        string? searchTerm = null,
        string? customerType = null,
        bool? isActive = null,
        DateTime? registeredFrom = null,
        DateTime? registeredTo = null,
        int page = 1,
        int pageSize = 10,
        string sortBy = "FullName",
        string sortDirection = "asc",
        CancellationToken cancellationToken = default)
    {
        var query = _context.Customers.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c =>
                c.FullName.Contains(searchTerm) ||
                c.CustomerCode.Contains(searchTerm) ||
                (c.Email != null && c.Email.Contains(searchTerm)) ||
                (c.PhoneNumber != null && c.PhoneNumber.Contains(searchTerm)) ||
                (c.CompanyName != null && c.CompanyName.Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(customerType))
        {
            query = query.Where(c => c.CustomerType == customerType);
        }

        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }

        if (registeredFrom.HasValue)
        {
            query = query.Where(c => c.RegisteredDate >= registeredFrom.Value);
        }

        if (registeredTo.HasValue)
        {
            query = query.Where(c => c.RegisteredDate <= registeredTo.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting (simplified)
        if (sortDirection?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true)
        {
            query = query.OrderByDescending(c => c.FullName);
        }
        else
        {
            query = query.OrderBy(c => c.FullName);
        }

        // Apply pagination
        var customers = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (customers, totalCount);
    }
}
