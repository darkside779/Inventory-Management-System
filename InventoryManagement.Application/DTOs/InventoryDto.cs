using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs;

/// <summary>
/// Data Transfer Object for Inventory entity
/// </summary>
public class InventoryDto
{
    /// <summary>
    /// Unique identifier for the inventory record
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
    /// Current quantity in stock
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int Quantity { get; set; }
    
    /// <summary>
    /// Reserved quantity (not available for sale)
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Reserved quantity cannot be negative")]
    public int ReservedQuantity { get; set; }
    
    /// <summary>
    /// Available quantity for sale (Quantity - ReservedQuantity)
    /// </summary>
    public int AvailableQuantity => Quantity - ReservedQuantity;
    
    /// <summary>
    /// Minimum stock level for this product in this warehouse
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Minimum stock level cannot be negative")]
    public int MinimumStockLevel { get; set; }
    
    /// <summary>
    /// Maximum stock level for this product in this warehouse
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Maximum stock level cannot be negative")]
    public int MaximumStockLevel { get; set; }
    
    /// <summary>
    /// Location within the warehouse (e.g., Aisle A, Shelf 1)
    /// </summary>
    [MaxLength(100)]
    public string? Location { get; set; }
    
    /// <summary>
    /// Indicates whether the inventory record is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Date and time when the inventory record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Date and time when the inventory record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Indicates if this inventory is below minimum stock level
    /// </summary>
    public bool IsBelowMinimum => Quantity < MinimumStockLevel;
    
    /// <summary>
    /// Indicates if this inventory is above maximum stock level
    /// </summary>
    public bool IsAboveMaximum => Quantity > MaximumStockLevel;
    
    /// <summary>
    /// Product unit price for value calculation
    /// </summary>
    public decimal ProductPrice { get; set; }
    
    /// <summary>
    /// Total value of inventory (Quantity * ProductPrice)
    /// </summary>
    public decimal TotalValue => Quantity * ProductPrice;
}

/// <summary>
/// DTO for creating a new inventory record
/// </summary>
public class CreateInventoryDto
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
    /// Initial quantity in stock
    /// </summary>
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int Quantity { get; set; }
    
    /// <summary>
    /// Reserved quantity (not available for sale)
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Reserved quantity cannot be negative")]
    public int ReservedQuantity { get; set; } = 0;
    
    /// <summary>
    /// Minimum stock level for this product in this warehouse
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Minimum stock level cannot be negative")]
    public int MinimumStockLevel { get; set; } = 0;
    
    /// <summary>
    /// Maximum stock level for this product in this warehouse
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Maximum stock level cannot be negative")]
    public int MaximumStockLevel { get; set; } = 1000;
    
    /// <summary>
    /// Location within the warehouse (e.g., Aisle A, Shelf 1)
    /// </summary>
    [MaxLength(100)]
    public string? Location { get; set; }
    
    /// <summary>
    /// Indicates whether the inventory record is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO for updating an existing inventory record
/// </summary>
public class UpdateInventoryDto
{
    /// <summary>
    /// Unique identifier for the inventory record
    /// </summary>
    [Required]
    public int Id { get; set; }
    
    /// <summary>
    /// Current quantity in stock
    /// </summary>
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int Quantity { get; set; }
    
    /// <summary>
    /// Reserved quantity (not available for sale)
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Reserved quantity cannot be negative")]
    public int ReservedQuantity { get; set; }
    
    /// <summary>
    /// Minimum stock level for this product in this warehouse
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Minimum stock level cannot be negative")]
    public int MinimumStockLevel { get; set; }
    
    /// <summary>
    /// Maximum stock level for this product in this warehouse
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Maximum stock level cannot be negative")]
    public int MaximumStockLevel { get; set; }
    
    /// <summary>
    /// Location within the warehouse (e.g., Aisle A, Shelf 1)
    /// </summary>
    [MaxLength(100)]
    public string? Location { get; set; }
    
    /// <summary>
    /// Indicates whether the inventory record is active
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for adjusting inventory quantity
/// </summary>
public class AdjustInventoryDto
{
    /// <summary>
    /// Unique identifier for the inventory record
    /// </summary>
    [Required]
    public int Id { get; set; }
    
    /// <summary>
    /// Quantity to adjust (positive for increase, negative for decrease)
    /// </summary>
    [Required]
    public int AdjustmentQuantity { get; set; }
    
    /// <summary>
    /// Reason for the adjustment
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
    
    /// <summary>
    /// Reference number for the adjustment
    /// </summary>
    [MaxLength(50)]
    public string? ReferenceNumber { get; set; }
}

/// <summary>
/// DTO for reserving inventory quantity
/// </summary>
public class ReserveInventoryDto
{
    /// <summary>
    /// Product ID
    /// </summary>
    [Required]
    public int ProductId { get; set; }
    
    /// <summary>
    /// Warehouse ID
    /// </summary>
    [Required]
    public int WarehouseId { get; set; }
    
    /// <summary>
    /// Quantity to reserve
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Reserve quantity must be greater than zero")]
    public int Quantity { get; set; }
    
    /// <summary>
    /// Reason for the reservation
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
    
    /// <summary>
    /// Reference number for the reservation
    /// </summary>
    [MaxLength(50)]
    public string? ReferenceNumber { get; set; }
}

/// <summary>
/// Simplified DTO for inventory lookups
/// </summary>
public class InventoryLookupDto
{
    /// <summary>
    /// Unique identifier for the inventory record
    /// </summary>
    public int Id { get; set; }
    
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
    /// Current quantity in stock
    /// </summary>
    public int Quantity { get; set; }
    
    /// <summary>
    /// Available quantity for sale
    /// </summary>
    public int AvailableQuantity { get; set; }
    
    /// <summary>
    /// Indicates if this inventory is below minimum stock level
    /// </summary>
    public bool IsBelowMinimum { get; set; }
}
