using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Application.Interfaces;

/// <summary>
/// Repository interface for Transaction entity with specific business operations
/// </summary>
public interface ITransactionRepository : IGenericRepository<Transaction>
{
    /// <summary>
    /// Get transactions by product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transactions for the product</returns>
    Task<IEnumerable<Transaction>> GetByProductAsync(int productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions by warehouse
    /// </summary>
    /// <param name="warehouseId">Warehouse ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transactions for the warehouse</returns>
    Task<IEnumerable<Transaction>> GetByWarehouseAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions by user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transactions by the user</returns>
    Task<IEnumerable<Transaction>> GetByUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions by type
    /// </summary>
    /// <param name="transactionType">Transaction type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transactions of the specified type</returns>
    Task<IEnumerable<Transaction>> GetByTypeAsync(TransactionType transactionType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions within date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transactions within the date range</returns>
    Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transaction by reference number
    /// </summary>
    /// <param name="referenceNumber">Reference number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transaction or null</returns>
    Task<Transaction?> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get recent transactions
    /// </summary>
    /// <param name="count">Number of transactions to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Recent transactions</returns>
    Task<IEnumerable<Transaction>> GetRecentTransactionsAsync(int count = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transaction history for a product in a warehouse
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="warehouseId">Warehouse ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transaction history</returns>
    Task<IEnumerable<Transaction>> GetProductWarehouseHistoryAsync(int productId, int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get stock movement summary for a date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Stock movement summary</returns>
    Task<(int StockInCount, int StockOutCount, int AdjustmentCount)> GetStockMovementSummaryAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
