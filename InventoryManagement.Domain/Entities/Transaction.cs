using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Domain.Entities;

/// <summary>
/// Represents an inventory transaction (stock movement)
/// </summary>
public class Transaction : BaseEntity
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
    /// User ID who created the transaction (foreign key)
    /// </summary>
    [Required]
    public int UserId { get; set; }
    
    /// <summary>
    /// Type of transaction (StockIn, StockOut, Adjustment)
    /// </summary>
    [Required]
    public TransactionType TransactionType { get; set; }
    
    /// <summary>
    /// Quantity changed (positive for stock in, negative for stock out)
    /// </summary>
    [Range(int.MinValue, int.MaxValue, ErrorMessage = "Quantity changed cannot be zero")]
    public int QuantityChanged { get; set; }
    
    /// <summary>
    /// Quantity before this transaction
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Previous quantity cannot be negative")]
    public int PreviousQuantity { get; set; }
    
    /// <summary>
    /// Quantity after this transaction
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "New quantity cannot be negative")]
    public int NewQuantity { get; set; }
    
    /// <summary>
    /// Unit cost at the time of transaction
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Unit cost cannot be negative")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal? UnitCost { get; set; }
    
    /// <summary>
    /// Total value of the transaction (calculated)
    /// </summary>
    [NotMapped]
    public decimal? TotalValue => UnitCost.HasValue ? Math.Abs(QuantityChanged) * UnitCost.Value : null;
    
    /// <summary>
    /// Reason for the transaction
    /// </summary>
    [MaxLength(200)]
    public string? Reason { get; set; }
    
    /// <summary>
    /// External reference number (PO, invoice, etc.)
    /// </summary>
    [MaxLength(50)]
    public string? ReferenceNumber { get; set; }
    
    /// <summary>
    /// Additional notes
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    /// <summary>
    /// Transaction timestamp (when the transaction occurred)
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    
    /// <summary>
    /// Product involved in this transaction
    /// </summary>
    [Required]
    public virtual Product Product { get; set; } = null!;
    
    /// <summary>
    /// Warehouse where the transaction occurred
    /// </summary>
    [Required]
    public virtual Warehouse Warehouse { get; set; } = null!;
    
    /// <summary>
    /// User who created the transaction
    /// </summary>
    [Required]
    public virtual User User { get; set; } = null!;
    
    // Domain methods
    
    /// <summary>
    /// Validates that the transaction data is consistent
    /// </summary>
    /// <returns>True if transaction is valid</returns>
    public bool IsValid()
    {
        // Basic validations
        if (QuantityChanged == 0) return false;
        if (PreviousQuantity < 0) return false;
        if (NewQuantity < 0) return false;
        if (UnitCost < 0) return false;
        
        // Validate quantity calculations
        var expectedNewQuantity = PreviousQuantity + QuantityChanged;
        if (NewQuantity != expectedNewQuantity) return false;
        
        // Validate transaction type consistency
        switch (TransactionType)
        {
            case TransactionType.StockIn:
                return QuantityChanged > 0;
            case TransactionType.StockOut:
                return QuantityChanged < 0;
            case TransactionType.Adjustment:
                return true; // Adjustments can be positive or negative
            default:
                return false;
        }
    }
    
    /// <summary>
    /// Gets the absolute quantity (always positive)
    /// </summary>
    /// <returns>Absolute quantity value</returns>
    public int GetAbsoluteQuantity()
    {
        return Math.Abs(QuantityChanged);
    }
    
    /// <summary>
    /// Checks if this is a stock increase transaction
    /// </summary>
    /// <returns>True if stock was increased</returns>
    public bool IsStockIncrease()
    {
        return QuantityChanged > 0;
    }
    
    /// <summary>
    /// Checks if this is a stock decrease transaction
    /// </summary>
    /// <returns>True if stock was decreased</returns>
    public bool IsStockDecrease()
    {
        return QuantityChanged < 0;
    }
    
    /// <summary>
    /// Gets a description of the transaction
    /// </summary>
    /// <returns>Human-readable transaction description</returns>
    public string GetDescription()
    {
        var action = TransactionType switch
        {
            TransactionType.StockIn => "Stock In",
            TransactionType.StockOut => "Stock Out",
            TransactionType.Adjustment => "Adjustment",
            _ => "Unknown"
        };
        
        var quantity = GetAbsoluteQuantity();
        var direction = IsStockIncrease() ? "increased" : "decreased";
        
        return $"{action}: Stock {direction} by {quantity}";
    }
    
    /// <summary>
    /// Creates a stock in transaction
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="warehouseId">Warehouse ID</param>
    /// <param name="userId">User ID</param>
    /// <param name="quantity">Quantity to add</param>
    /// <param name="previousQuantity">Previous stock quantity</param>
    /// <param name="unitCost">Unit cost</param>
    /// <param name="reason">Reason for stock in</param>
    /// <param name="referenceNumber">Reference number</param>
    /// <returns>New stock in transaction</returns>
    public static Transaction CreateStockIn(
        int productId,
        int warehouseId,
        int userId,
        int quantity,
        int previousQuantity,
        decimal? unitCost = null,
        string? reason = null,
        string? referenceNumber = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("Stock in quantity must be positive", nameof(quantity));
            
        return new Transaction
        {
            ProductId = productId,
            WarehouseId = warehouseId,
            UserId = userId,
            TransactionType = TransactionType.StockIn,
            QuantityChanged = quantity,
            PreviousQuantity = previousQuantity,
            NewQuantity = previousQuantity + quantity,
            UnitCost = unitCost,
            Reason = reason,
            ReferenceNumber = referenceNumber,
            Timestamp = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Creates a stock out transaction
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="warehouseId">Warehouse ID</param>
    /// <param name="userId">User ID</param>
    /// <param name="quantity">Quantity to remove</param>
    /// <param name="previousQuantity">Previous stock quantity</param>
    /// <param name="unitCost">Unit cost</param>
    /// <param name="reason">Reason for stock out</param>
    /// <param name="referenceNumber">Reference number</param>
    /// <returns>New stock out transaction</returns>
    public static Transaction CreateStockOut(
        int productId,
        int warehouseId,
        int userId,
        int quantity,
        int previousQuantity,
        decimal? unitCost = null,
        string? reason = null,
        string? referenceNumber = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("Stock out quantity must be positive", nameof(quantity));
            
        if (previousQuantity < quantity)
            throw new ArgumentException("Insufficient stock for stock out operation");
            
        return new Transaction
        {
            ProductId = productId,
            WarehouseId = warehouseId,
            UserId = userId,
            TransactionType = TransactionType.StockOut,
            QuantityChanged = -quantity,
            PreviousQuantity = previousQuantity,
            NewQuantity = previousQuantity - quantity,
            UnitCost = unitCost,
            Reason = reason,
            ReferenceNumber = referenceNumber,
            Timestamp = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Creates an adjustment transaction
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="warehouseId">Warehouse ID</param>
    /// <param name="userId">User ID</param>
    /// <param name="quantityChange">Quantity change (positive or negative)</param>
    /// <param name="previousQuantity">Previous stock quantity</param>
    /// <param name="reason">Reason for adjustment</param>
    /// <param name="referenceNumber">Reference number</param>
    /// <returns>New adjustment transaction</returns>
    public static Transaction CreateAdjustment(
        int productId,
        int warehouseId,
        int userId,
        int quantityChange,
        int previousQuantity,
        string? reason = null,
        string? referenceNumber = null)
    {
        if (quantityChange == 0)
            throw new ArgumentException("Adjustment quantity cannot be zero", nameof(quantityChange));
            
        var newQuantity = previousQuantity + quantityChange;
        if (newQuantity < 0)
            throw new ArgumentException("Adjustment would result in negative stock");
            
        return new Transaction
        {
            ProductId = productId,
            WarehouseId = warehouseId,
            UserId = userId,
            TransactionType = TransactionType.Adjustment,
            QuantityChanged = quantityChange,
            PreviousQuantity = previousQuantity,
            NewQuantity = newQuantity,
            Reason = reason,
            ReferenceNumber = referenceNumber,
            Timestamp = DateTime.UtcNow
        };
    }
}