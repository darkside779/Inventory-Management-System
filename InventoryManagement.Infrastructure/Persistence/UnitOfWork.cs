using Microsoft.EntityFrameworkCore.Storage;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Infrastructure.Persistence.Repositories;

namespace InventoryManagement.Infrastructure.Persistence;

/// <summary>
/// Unit of Work implementation for managing repository transactions
/// </summary>
public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    // Repository instances - lazy initialization
    private ICategoryRepository? _categories;
    private IWarehouseRepository? _warehouses;
    private ISupplierRepository? _suppliers;
    private IUserRepository? _users;
    private IProductRepository? _products;
    private IInventoryRepository? _inventory;
    private ITransactionRepository? _transactions;

    public UnitOfWork(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Category repository
    /// </summary>
    public ICategoryRepository Categories
    {
        get
        {
            _categories ??= new CategoryRepository(_context);
            return _categories;
        }
    }

    /// <summary>
    /// Warehouse repository
    /// </summary>
    public IWarehouseRepository Warehouses
    {
        get
        {
            _warehouses ??= new WarehouseRepository(_context);
            return _warehouses;
        }
    }

    /// <summary>
    /// Supplier repository
    /// </summary>
    public ISupplierRepository Suppliers
    {
        get
        {
            _suppliers ??= new SupplierRepository(_context);
            return _suppliers;
        }
    }

    /// <summary>
    /// User repository
    /// </summary>
    public IUserRepository Users
    {
        get
        {
            _users ??= new UserRepository(_context);
            return _users;
        }
    }

    /// <summary>
    /// Product repository
    /// </summary>
    public IProductRepository Products
    {
        get
        {
            _products ??= new ProductRepository(_context);
            return _products;
        }
    }

    /// <summary>
    /// Inventory repository
    /// </summary>
    public IInventoryRepository Inventory
    {
        get
        {
            _inventory ??= new InventoryRepository(_context);
            return _inventory;
        }
    }

    /// <summary>
    /// Transaction repository
    /// </summary>
    public ITransactionRepository Transactions
    {
        get
        {
            _transactions ??= new TransactionRepository(_context);
            return _transactions;
        }
    }

    /// <summary>
    /// Save all changes to the database
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Begin database transaction
    /// </summary>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Commit current transaction
    /// </summary>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction is in progress.");
        }

        try
        {
            await _transaction.CommitAsync(cancellationToken);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Rollback current transaction
    /// </summary>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction is in progress.");
        }

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Dispose resources
    /// </summary>
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
