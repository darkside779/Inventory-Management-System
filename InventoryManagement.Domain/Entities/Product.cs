using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Entities;

/// <summary>
/// Represents a product in the inventory system
/// </summary>
public class Product : BaseEntity
{
    /// <summary>
    /// Name of the product
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Stock Keeping Unit - unique identifier for the product
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string SKU { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed description of the product
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Selling price of the product
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    
    /// <summary>
    /// Cost price of the product
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Cost cannot be negative")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Cost { get; set; }
    
    /// <summary>
    /// Category ID (foreign key)
    /// </summary>
    [Required]
    public int CategoryId { get; set; }
    
    /// <summary>
    /// Supplier ID (foreign key) - optional
    /// </summary>
    public int? SupplierId { get; set; }
    
    /// <summary>
    /// Low stock alert threshold
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Low stock threshold cannot be negative")]
    public int LowStockThreshold { get; set; } = 10;
    
    /// <summary>
    /// Unit of measurement (e.g., "Piece", "Kg", "Liter")
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Unit { get; set; } = "Piece";
    
    /// <summary>
    /// Product barcode
    /// </summary>
    [MaxLength(100)]
    public string? Barcode { get; set; }
    
    /// <summary>
    /// Product weight
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Weight cannot be negative")]
    [Column(TypeName = "decimal(10,3)")]
    public decimal? Weight { get; set; }
    
    /// <summary>
    /// Product dimensions (length x width x height)
    /// </summary>
    [MaxLength(100)]
    public string? Dimensions { get; set; }
    
    /// <summary>
    /// Indicates whether the product is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    
    /// <summary>
    /// Category this product belongs to
    /// </summary>
    [Required]
    public virtual Category Category { get; set; } = null!;
    
    /// <summary>
    /// Supplier of this product (optional)
    /// </summary>
    public virtual Supplier? Supplier { get; set; }
    
    /// <summary>
    /// Inventory records for this product across warehouses
    /// </summary>
    public virtual ICollection<Inventory> InventoryItems { get; set; } = new List<Inventory>();
    
    /// <summary>
    /// Transactions involving this product
    /// </summary>
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    
    // Domain methods
    
    /// <summary>
    /// Checks if the product is low on stock based on total inventory
    /// </summary>
    /// <param name="totalQuantity">Total quantity across all warehouses</param>
    /// <returns>True if stock is below threshold</returns>
    public bool IsLowStock(int totalQuantity)
    {
        return totalQuantity <= LowStockThreshold;
    }
    
    /// <summary>
    /// Calculates the profit margin percentage
    /// </summary>
    /// <returns>Profit margin as percentage, or null if cost is not set</returns>
    public decimal? GetProfitMarginPercentage()
    {
        if (Cost == null || Cost == 0) return null;
        return ((Price - Cost.Value) / Cost.Value) * 100;
    }
}