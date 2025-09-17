using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Domain.Entities;

/// <summary>
/// Represents a warehouse or storage location
/// </summary>
public class Warehouse : BaseEntity
{
    /// <summary>
    /// Name of the warehouse
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// General location of the warehouse
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;
    
    /// <summary>
    /// Full address of the warehouse
    /// </summary>
    [MaxLength(500)]
    public string? Address { get; set; }
    
    /// <summary>
    /// Contact phone number for the warehouse
    /// </summary>
    [MaxLength(20)]
    public string? ContactPhone { get; set; }
    
    /// <summary>
    /// Contact email for the warehouse
    /// </summary>
    [MaxLength(100)]
    [EmailAddress]
    public string? ContactEmail { get; set; }
    
    /// <summary>
    /// Storage capacity of the warehouse
    /// </summary>
    [Range(0, int.MaxValue)]
    public int? Capacity { get; set; }
    
    /// <summary>
    /// Indicates whether the warehouse is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    
    /// <summary>
    /// Inventory items stored in this warehouse
    /// </summary>
    public virtual ICollection<Inventory> InventoryItems { get; set; } = new List<Inventory>();
    
    /// <summary>
    /// Transactions that occurred in this warehouse
    /// </summary>
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}