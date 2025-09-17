using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs;

/// <summary>
/// Dashboard summary data
/// </summary>
public class DashboardSummaryDto
{
    /// <summary>
    /// Total number of products
    /// </summary>
    public int TotalProducts { get; set; }

    /// <summary>
    /// Total number of active inventory items
    /// </summary>
    public int TotalInventoryItems { get; set; }

    /// <summary>
    /// Total inventory value
    /// </summary>
    public decimal TotalInventoryValue { get; set; }

    /// <summary>
    /// Number of low stock items
    /// </summary>
    public int LowStockItems { get; set; }

    /// <summary>
    /// Number of out of stock items
    /// </summary>
    public int OutOfStockItems { get; set; }

    /// <summary>
    /// Total transactions today
    /// </summary>
    public int TransactionsToday { get; set; }

    /// <summary>
    /// Total transactions this month
    /// </summary>
    public int TransactionsThisMonth { get; set; }

    /// <summary>
    /// Stock in transactions today
    /// </summary>
    public int StockInToday { get; set; }

    /// <summary>
    /// Stock out transactions today
    /// </summary>
    public int StockOutToday { get; set; }

    /// <summary>
    /// Total value of stock movements today
    /// </summary>
    public decimal StockMovementValueToday { get; set; }
}

/// <summary>
/// Inventory report data
/// </summary>
public class InventoryReportDto
{
    /// <summary>
    /// Product ID
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Product name
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Product SKU
    /// </summary>
    public string ProductSKU { get; set; } = string.Empty;

    /// <summary>
    /// Category name
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Warehouse ID
    /// </summary>
    public int WarehouseId { get; set; }

    /// <summary>
    /// Warehouse name
    /// </summary>
    public string WarehouseName { get; set; } = string.Empty;

    /// <summary>
    /// Current quantity
    /// </summary>
    public int CurrentQuantity { get; set; }

    /// <summary>
    /// Reserved quantity
    /// </summary>
    public int ReservedQuantity { get; set; }

    /// <summary>
    /// Available quantity
    /// </summary>
    public int AvailableQuantity => CurrentQuantity - ReservedQuantity;

    /// <summary>
    /// Minimum stock level
    /// </summary>
    public int MinimumStockLevel { get; set; }

    /// <summary>
    /// Maximum stock level
    /// </summary>
    public int MaximumStockLevel { get; set; }

    /// <summary>
    /// Unit price
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Total value
    /// </summary>
    public decimal TotalValue => CurrentQuantity * UnitPrice;

    /// <summary>
    /// Stock status
    /// </summary>
    public string StockStatus
    {
        get
        {
            if (CurrentQuantity <= 0) return "Out of Stock";
            if (CurrentQuantity <= MinimumStockLevel) return "Low Stock";
            if (CurrentQuantity >= MaximumStockLevel) return "Overstock";
            return "Normal";
        }
    }

    /// <summary>
    /// Last stock count date
    /// </summary>
    public DateTime? LastStockCount { get; set; }
}

/// <summary>
/// Transaction report data
/// </summary>
public class TransactionReportDto
{
    /// <summary>
    /// Transaction ID
    /// </summary>
    public int TransactionId { get; set; }

    /// <summary>
    /// Transaction date
    /// </summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// Transaction type
    /// </summary>
    public string TransactionType { get; set; } = string.Empty;

    /// <summary>
    /// Product name
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Product SKU
    /// </summary>
    public string ProductSKU { get; set; } = string.Empty;

    /// <summary>
    /// Warehouse name
    /// </summary>
    public string WarehouseName { get; set; } = string.Empty;

    /// <summary>
    /// Quantity changed
    /// </summary>
    public int QuantityChanged { get; set; }

    /// <summary>
    /// Unit cost
    /// </summary>
    public decimal? UnitCost { get; set; }

    /// <summary>
    /// Total value
    /// </summary>
    public decimal TotalValue => Math.Abs(QuantityChanged) * (UnitCost ?? 0);

    /// <summary>
    /// User name
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Reference number
    /// </summary>
    public string? ReferenceNumber { get; set; }

    /// <summary>
    /// Reason for transaction
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Previous quantity before transaction
    /// </summary>
    public int PreviousQuantity { get; set; }

    /// <summary>
    /// New quantity after transaction
    /// </summary>
    public int NewQuantity { get; set; }
}

/// <summary>
/// Product movement analysis
/// </summary>
public class ProductMovementReportDto
{
    /// <summary>
    /// Product ID
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Product name
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Product SKU
    /// </summary>
    public string ProductSKU { get; set; } = string.Empty;

    /// <summary>
    /// Category name
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Total stock in quantity
    /// </summary>
    public int TotalStockIn { get; set; }

    /// <summary>
    /// Total stock out quantity
    /// </summary>
    public int TotalStockOut { get; set; }

    /// <summary>
    /// Net movement (stock in - stock out)
    /// </summary>
    public int NetMovement => TotalStockIn - TotalStockOut;

    /// <summary>
    /// Total stock in value
    /// </summary>
    public decimal TotalStockInValue { get; set; }

    /// <summary>
    /// Total stock out value
    /// </summary>
    public decimal TotalStockOutValue { get; set; }

    /// <summary>
    /// Number of transactions
    /// </summary>
    public int TransactionCount { get; set; }

    /// <summary>
    /// Average transaction size
    /// </summary>
    public decimal AverageTransactionSize => TransactionCount > 0 ? (decimal)(TotalStockIn + TotalStockOut) / TransactionCount : 0;

    /// <summary>
    /// First transaction date
    /// </summary>
    public DateTime? FirstTransactionDate { get; set; }

    /// <summary>
    /// Last transaction date
    /// </summary>
    public DateTime? LastTransactionDate { get; set; }

    /// <summary>
    /// Current stock level
    /// </summary>
    public int CurrentStockLevel { get; set; }

    /// <summary>
    /// Movement velocity (transactions per day)
    /// </summary>
    public decimal MovementVelocity
    {
        get
        {
            if (!FirstTransactionDate.HasValue || !LastTransactionDate.HasValue) return 0;
            var days = (LastTransactionDate.Value - FirstTransactionDate.Value).TotalDays;
            return days > 0 ? TransactionCount / (decimal)days : 0;
        }
    }
}

/// <summary>
/// Stock level trend data
/// </summary>
public class StockLevelTrendDto
{
    /// <summary>
    /// Date
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Total stock value
    /// </summary>
    public decimal TotalStockValue { get; set; }

    /// <summary>
    /// Total stock quantity
    /// </summary>
    public int TotalStockQuantity { get; set; }

    /// <summary>
    /// Number of stock in transactions
    /// </summary>
    public int StockInTransactions { get; set; }

    /// <summary>
    /// Number of stock out transactions
    /// </summary>
    public int StockOutTransactions { get; set; }

    /// <summary>
    /// Stock in quantity
    /// </summary>
    public int StockInQuantity { get; set; }

    /// <summary>
    /// Stock out quantity
    /// </summary>
    public int StockOutQuantity { get; set; }

    /// <summary>
    /// Net change in stock
    /// </summary>
    public int NetStockChange => StockInQuantity - StockOutQuantity;
}

/// <summary>
/// Warehouse performance report
/// </summary>
public class WarehousePerformanceReportDto
{
    /// <summary>
    /// Warehouse ID
    /// </summary>
    public int WarehouseId { get; set; }

    /// <summary>
    /// Warehouse name
    /// </summary>
    public string WarehouseName { get; set; } = string.Empty;

    /// <summary>
    /// Total inventory items
    /// </summary>
    public int TotalInventoryItems { get; set; }

    /// <summary>
    /// Total inventory value
    /// </summary>
    public decimal TotalInventoryValue { get; set; }

    /// <summary>
    /// Number of transactions
    /// </summary>
    public int TotalTransactions { get; set; }

    /// <summary>
    /// Low stock items count
    /// </summary>
    public int LowStockItems { get; set; }

    /// <summary>
    /// Out of stock items count
    /// </summary>
    public int OutOfStockItems { get; set; }

    /// <summary>
    /// Overstock items count
    /// </summary>
    public int OverstockItems { get; set; }

    /// <summary>
    /// Stock turnover rate
    /// </summary>
    public decimal StockTurnoverRate { get; set; }

    /// <summary>
    /// Average transaction value
    /// </summary>
    public decimal AverageTransactionValue { get; set; }

    /// <summary>
    /// Warehouse utilization percentage
    /// </summary>
    public decimal UtilizationPercentage { get; set; }
}

/// <summary>
/// Report filter parameters
/// </summary>
public class ReportFilterDto
{
    /// <summary>
    /// Start date for filtering
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date for filtering
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Product IDs to include
    /// </summary>
    public List<int>? ProductIds { get; set; }

    /// <summary>
    /// Warehouse IDs to include
    /// </summary>
    public List<int>? WarehouseIds { get; set; }

    /// <summary>
    /// Category IDs to include
    /// </summary>
    public List<int>? CategoryIds { get; set; }

    /// <summary>
    /// User IDs to include
    /// </summary>
    public List<int>? UserIds { get; set; }

    /// <summary>
    /// Transaction types to include
    /// </summary>
    public List<string>? TransactionTypes { get; set; }

    /// <summary>
    /// Include low stock items only
    /// </summary>
    public bool? LowStockOnly { get; set; }

    /// <summary>
    /// Include out of stock items only
    /// </summary>
    public bool? OutOfStockOnly { get; set; }

    /// <summary>
    /// Minimum value threshold
    /// </summary>
    public decimal? MinValue { get; set; }

    /// <summary>
    /// Maximum value threshold
    /// </summary>
    public decimal? MaxValue { get; set; }
}
