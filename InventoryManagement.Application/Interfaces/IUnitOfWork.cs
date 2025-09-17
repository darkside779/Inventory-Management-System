namespace InventoryManagement.Application.Interfaces;

/// <summary>
/// Unit of Work interface for managing repository transactions
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Category repository
    /// </summary>
    ICategoryRepository Categories { get; }

    /// <summary>
    /// Warehouse repository
    /// </summary>
    IWarehouseRepository Warehouses { get; }

    /// <summary>
    /// Supplier repository
    /// </summary>
    ISupplierRepository Suppliers { get; }

    /// <summary>
    /// User repository
    /// </summary>
    IUserRepository Users { get; }

    /// <summary>
    /// Product repository
    /// </summary>
    IProductRepository Products { get; }

    /// <summary>
    /// Inventory repository
    /// </summary>
    IInventoryRepository Inventory { get; }

    /// <summary>
    /// Transaction repository
    /// </summary>
    ITransactionRepository Transactions { get; }

    /// <summary>
    /// Save all changes to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of affected rows</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begin database transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Database transaction</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commit current transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rollback current transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
