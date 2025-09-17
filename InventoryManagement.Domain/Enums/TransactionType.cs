namespace InventoryManagement.Domain.Enums;

/// <summary>
/// Represents the different types of inventory transactions
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// Stock received from supplier or transfer
    /// </summary>
    StockIn = 1,
    
    /// <summary>
    /// Stock sold, shipped, or transferred out
    /// </summary>
    StockOut = 2,
    
    /// <summary>
    /// Manual stock adjustment (corrections, damage, etc.)
    /// </summary>
    Adjustment = 3
}