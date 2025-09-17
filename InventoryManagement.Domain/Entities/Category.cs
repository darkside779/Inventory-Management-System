using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Domain.Entities;

/// <summary>
/// Represents a product category in the inventory system
/// </summary>
public class Category : BaseEntity
{
    /// <summary>
    /// Name of the category
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the category
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Indicates whether the category is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    
    /// <summary>
    /// Products belonging to this category
    /// </summary>
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}