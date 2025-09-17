using System.ComponentModel.DataAnnotations;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Application.DTOs;

/// <summary>
/// Data Transfer Object for Transaction entity
/// </summary>
public class TransactionDto
{
    /// <summary>
    /// Unique identifier for the transaction
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Product ID (foreign key)
    /// </summary>
    [Required]
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
    /// Warehouse ID (foreign key)
    /// </summary>
    [Required]
    public int WarehouseId { get; set; }
    
    /// <summary>
    /// Warehouse name
    /// </summary>
    public string WarehouseName { get; set; } = string.Empty;
    
    /// <summary>
    /// User ID who performed the transaction (foreign key)
    /// </summary>
    [Required]
    public int UserId { get; set; }
    
    /// <summary>
    /// User name who performed the transaction
    /// </summary>
    public string UserName { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of transaction (In, Out, Adjustment)
    /// </summary>
    [Required]
    public TransactionType Type { get; set; }
    
    /// <summary>
    /// Quantity involved in the transaction (positive for In/Adjustment up, negative for Out/Adjustment down)
    /// </summary>
    [Required]
    public int Quantity { get; set; }
    
    /// <summary>
    /// Unit price at the time of transaction
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Unit price cannot be negative")]
    public decimal? UnitPrice { get; set; }
    
    /// <summary>
    /// Total value of the transaction (Quantity * UnitPrice)
    /// </summary>
    public decimal? TotalValue => UnitPrice.HasValue ? Math.Abs(Quantity) * UnitPrice.Value : null;
    
    /// <summary>
    /// Date and time when the transaction occurred
    /// </summary>
    [Required]
    public DateTime TransactionDate { get; set; }
    
    /// <summary>
    /// Reference number for the transaction (e.g., PO number, invoice number)
    /// </summary>
    [MaxLength(100)]
    public string? ReferenceNumber { get; set; }
    
    /// <summary>
    /// Additional notes about the transaction
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    /// <summary>
    /// Indicates whether the transaction is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Date and time when the transaction record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Date and time when the transaction record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new transaction
/// </summary>
public class CreateTransactionDto
{
    /// <summary>
    /// Product ID (foreign key)
    /// </summary>
    [Required]
    public int ProductId { get; set; }
    
    /// <summary>
    /// Warehouse ID (foreign key)
    /// </summary>
    [Required]
    public int WarehouseId { get; set; }
    
    /// <summary>
    /// User ID who performed the transaction (foreign key)
    /// </summary>
    [Required]
    public int UserId { get; set; }
    
    /// <summary>
    /// Type of transaction (In, Out, Adjustment)
    /// </summary>
    [Required]
    public TransactionType Type { get; set; }
    
    /// <summary>
    /// Quantity involved in the transaction
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
    public int Quantity { get; set; }
    
    /// <summary>
    /// Unit price at the time of transaction
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Unit price cannot be negative")]
    public decimal? UnitPrice { get; set; }
    
    /// <summary>
    /// Date and time when the transaction occurred
    /// </summary>
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Reference number for the transaction (e.g., PO number, invoice number)
    /// </summary>
    [MaxLength(100)]
    public string? ReferenceNumber { get; set; }
    
    /// <summary>
    /// Additional notes about the transaction
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    /// <summary>
    /// Indicates whether the transaction is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO for updating an existing transaction
/// </summary>
public class UpdateTransactionDto
{
    /// <summary>
    /// Unique identifier for the transaction
    /// </summary>
    [Required]
    public int Id { get; set; }
    
    /// <summary>
    /// Type of transaction (In, Out, Adjustment)
    /// </summary>
    [Required]
    public TransactionType Type { get; set; }
    
    /// <summary>
    /// Quantity involved in the transaction
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
    public int Quantity { get; set; }
    
    /// <summary>
    /// Unit price at the time of transaction
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Unit price cannot be negative")]
    public decimal? UnitPrice { get; set; }
    
    /// <summary>
    /// Date and time when the transaction occurred
    /// </summary>
    [Required]
    public DateTime TransactionDate { get; set; }
    
    /// <summary>
    /// Reference number for the transaction (e.g., PO number, invoice number)
    /// </summary>
    [MaxLength(100)]
    public string? ReferenceNumber { get; set; }
    
    /// <summary>
    /// Additional notes about the transaction
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    /// <summary>
    /// Indicates whether the transaction is active
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for stock movement operations
/// </summary>
public class StockMovementDto
{
    /// <summary>
    /// Product ID
    /// </summary>
    [Required]
    public int ProductId { get; set; }
    
    /// <summary>
    /// Source warehouse ID (for transfers, null for stock in)
    /// </summary>
    public int? SourceWarehouseId { get; set; }
    
    /// <summary>
    /// Destination warehouse ID (for transfers, null for stock out)
    /// </summary>
    public int? DestinationWarehouseId { get; set; }
    
    /// <summary>
    /// Quantity to move
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
    public int Quantity { get; set; }
    
    /// <summary>
    /// Type of movement
    /// </summary>
    [Required]
    public TransactionType MovementType { get; set; }
    
    /// <summary>
    /// Unit price for the movement
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Unit price cannot be negative")]
    public decimal? UnitPrice { get; set; }
    
    /// <summary>
    /// Reference number for the movement
    /// </summary>
    [MaxLength(100)]
    public string? ReferenceNumber { get; set; }
    
    /// <summary>
    /// Notes about the movement
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    /// <summary>
    /// User performing the movement
    /// </summary>
    [Required]
    public int UserId { get; set; }
}

/// <summary>
/// DTO for transaction summary reports
/// </summary>
public class TransactionSummaryDto
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
    /// Warehouse ID
    /// </summary>
    public int WarehouseId { get; set; }
    
    /// <summary>
    /// Warehouse name
    /// </summary>
    public string WarehouseName { get; set; } = string.Empty;
    
    /// <summary>
    /// Total stock in quantity
    /// </summary>
    public int TotalStockIn { get; set; }
    
    /// <summary>
    /// Total stock out quantity
    /// </summary>
    public int TotalStockOut { get; set; }
    
    /// <summary>
    /// Total adjustments quantity
    /// </summary>
    public int TotalAdjustments { get; set; }
    
    /// <summary>
    /// Net movement (StockIn - StockOut + Adjustments)
    /// </summary>
    public int NetMovement => TotalStockIn - TotalStockOut + TotalAdjustments;
    
    /// <summary>
    /// Total value of stock in
    /// </summary>
    public decimal TotalStockInValue { get; set; }
    
    /// <summary>
    /// Total value of stock out
    /// </summary>
    public decimal TotalStockOutValue { get; set; }
    
    /// <summary>
    /// Start date of the summary period
    /// </summary>
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// End date of the summary period
    /// </summary>
    public DateTime EndDate { get; set; }
}

/// <summary>
/// Simplified DTO for transaction lookups
/// </summary>
public class TransactionLookupDto
{
    /// <summary>
    /// Unique identifier for the transaction
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Product name
    /// </summary>
    public string ProductName { get; set; } = string.Empty;
    
    /// <summary>
    /// Warehouse name
    /// </summary>
    public string WarehouseName { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of transaction
    /// </summary>
    public TransactionType Type { get; set; }
    
    /// <summary>
    /// Quantity involved in the transaction
    /// </summary>
    public int Quantity { get; set; }
    
    /// <summary>
    /// Date and time when the transaction occurred
    /// </summary>
    public DateTime TransactionDate { get; set; }
    
    /// <summary>
    /// Reference number for the transaction
    /// </summary>
    public string? ReferenceNumber { get; set; }
    
    /// <summary>
    /// User who performed the transaction
    /// </summary>
    public string UserName { get; set; } = string.Empty;
}
