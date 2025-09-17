using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces;

/// <summary>
/// Repository interface for Supplier entity with specific business operations
/// </summary>
public interface ISupplierRepository : IGenericRepository<Supplier>
{
    /// <summary>
    /// Get supplier by name
    /// </summary>
    /// <param name="name">Supplier name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Supplier or null</returns>
    Task<Supplier?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search suppliers by name or contact info
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Matching suppliers</returns>
    Task<IEnumerable<Supplier>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get suppliers with product counts
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Suppliers with product counts</returns>
    Task<IEnumerable<(Supplier Supplier, int ProductCount)>> GetSuppliersWithProductCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if supplier name is unique
    /// </summary>
    /// <param name="name">Supplier name</param>
    /// <param name="excludeSupplierId">Supplier ID to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if name is unique</returns>
    Task<bool> IsNameUniqueAsync(string name, int? excludeSupplierId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active suppliers only
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Active suppliers</returns>
    Task<IEnumerable<Supplier>> GetActiveSuppliersAsync(CancellationToken cancellationToken = default);
}
