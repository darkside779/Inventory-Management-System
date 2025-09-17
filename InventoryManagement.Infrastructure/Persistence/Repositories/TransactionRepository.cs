using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// Transaction repository implementation with specific business operations
/// </summary>
public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
{
    public TransactionRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get transactions by product
    /// </summary>
    public async Task<IEnumerable<Transaction>> GetByProductAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.ProductId == productId)
            .Include(t => t.Product)
            .Include(t => t.Warehouse)
            .Include(t => t.User)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get transactions by warehouse
    /// </summary>
    public async Task<IEnumerable<Transaction>> GetByWarehouseAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.WarehouseId == warehouseId)
            .Include(t => t.Product)
            .Include(t => t.Warehouse)
            .Include(t => t.User)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get transactions by user
    /// </summary>
    public async Task<IEnumerable<Transaction>> GetByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.UserId == userId)
            .Include(t => t.Product)
            .Include(t => t.Warehouse)
            .Include(t => t.User)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get transactions by type
    /// </summary>
    public async Task<IEnumerable<Transaction>> GetByTypeAsync(TransactionType transactionType, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.TransactionType == transactionType)
            .Include(t => t.Product)
            .Include(t => t.Warehouse)
            .Include(t => t.User)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get transactions within date range
    /// </summary>
    public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.Timestamp >= startDate && t.Timestamp <= endDate)
            .Include(t => t.Product)
            .Include(t => t.Warehouse)
            .Include(t => t.User)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get transaction by reference number
    /// </summary>
    public async Task<Transaction?> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Product)
            .Include(t => t.Warehouse)
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.ReferenceNumber == referenceNumber, cancellationToken);
    }

    /// <summary>
    /// Get recent transactions
    /// </summary>
    public async Task<IEnumerable<Transaction>> GetRecentTransactionsAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Product)
            .Include(t => t.Warehouse)
            .Include(t => t.User)
            .OrderByDescending(t => t.Timestamp)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get transaction history for a product in a warehouse
    /// </summary>
    public async Task<IEnumerable<Transaction>> GetProductWarehouseHistoryAsync(int productId, int warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.ProductId == productId && t.WarehouseId == warehouseId)
            .Include(t => t.Product)
            .Include(t => t.Warehouse)
            .Include(t => t.User)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get stock movement summary for a date range
    /// </summary>
    public async Task<(int StockInCount, int StockOutCount, int AdjustmentCount)> GetStockMovementSummaryAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var transactions = await _dbSet
            .Where(t => t.Timestamp >= startDate && t.Timestamp <= endDate)
            .ToListAsync(cancellationToken);

        var stockInCount = transactions.Count(t => t.TransactionType == TransactionType.StockIn);
        var stockOutCount = transactions.Count(t => t.TransactionType == TransactionType.StockOut);
        var adjustmentCount = transactions.Count(t => t.TransactionType == TransactionType.Adjustment);

        return (stockInCount, stockOutCount, adjustmentCount);
    }
}
