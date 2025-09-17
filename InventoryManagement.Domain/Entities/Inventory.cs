using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Entities;

/// <summary>
/// Represents inventory levels for a product in a specific warehouse
/// </summary>
public class Inventory : BaseEntity
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
    /// Current quantity in stock
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int Quantity { get; set; } = 0;
    
    /// <summary>
    /// Quantity reserved for pending orders
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Reserved quantity cannot be negative")]
    public int ReservedQuantity { get; set; } = 0;
    
    /// <summary>
    /// Available quantity for sale (calculated)
    /// </summary>
    [NotMapped]
    public int AvailableQuantity => Quantity - ReservedQuantity;
    
    /// <summary>
    /// Date of last physical stock count
    /// </summary>
    public DateTime? LastStockCount { get; set; }
    
    // Navigation properties
    
    /// <summary>
    /// Product associated with this inventory record
    /// </summary>
    [Required]
    public virtual Product Product { get; set; } = null!;
    
    /// <summary>
    /// Warehouse where this inventory is stored
    /// </summary>
    [Required]
    public virtual Warehouse Warehouse { get; set; } = null!;
    
    // Domain methods
    
    /// <summary>
    /// Checks if the inventory level is below the product's low stock threshold
    /// </summary>
    /// <returns>True if stock is low</returns>
    public bool IsLowStock()
    {
        return Product != null && Quantity <= Product.LowStockThreshold;
    }
    
    /// <summary>
    /// Checks if there is sufficient available quantity for a requested amount
    /// </summary>
    /// <param name="requestedQuantity">Quantity requested</param>
    /// <returns>True if sufficient stock is available</returns>
    public bool HasSufficientStock(int requestedQuantity)
    {
        return AvailableQuantity >= requestedQuantity;
    }
    
    /// <summary>
    /// Reserves a specified quantity for pending orders
    /// </summary>
    /// <param name="quantityToReserve">Quantity to reserve</param>
    /// <returns>True if reservation was successful</returns>
    public bool ReserveQuantity(int quantityToReserve)
    {
        if (quantityToReserve < 0)
            throw new ArgumentException("Cannot reserve negative quantity", nameof(quantityToReserve));
            
        if (!HasSufficientStock(quantityToReserve))
            return false;
            
        ReservedQuantity += quantityToReserve;
        UpdatedAt = DateTime.UtcNow;
        return true;
    }
    
    /// <summary>
    /// Releases reserved quantity back to available stock
    /// </summary>
    /// <param name="quantityToRelease">Quantity to release</param>
    /// <returns>True if release was successful</returns>
    public bool ReleaseReservedQuantity(int quantityToRelease)
    {
        if (quantityToRelease < 0)
            throw new ArgumentException("Cannot release negative quantity", nameof(quantityToRelease));
            
        if (quantityToRelease > ReservedQuantity)
            return false;
            
        ReservedQuantity -= quantityToRelease;
        UpdatedAt = DateTime.UtcNow;
        return true;
    }
    
    /// <summary>
    /// Adjusts the total quantity (used for stock in/out operations)
    /// </summary>
    /// <param name="quantityChange">Positive for stock in, negative for stock out</param>
    /// <returns>True if adjustment was successful</returns>
    public bool AdjustQuantity(int quantityChange)
    {
        var newQuantity = Quantity + quantityChange;
        
        if (newQuantity < 0)
            return false;
            
        if (newQuantity < ReservedQuantity)
            return false; // Cannot have less total quantity than reserved
            
        Quantity = newQuantity;
        UpdatedAt = DateTime.UtcNow;
        return true;
    }
    
    /// <summary>
    /// Validates that reserved quantity does not exceed total quantity
    /// </summary>
    /// <returns>True if inventory state is valid</returns>
    public bool IsValid()
    {
        return Quantity >= 0 && 
               ReservedQuantity >= 0 && 
               ReservedQuantity <= Quantity;
    }
}