using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Domain.Entities;

/// <summary>
/// Base entity class containing common properties for all entities
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Date and time when the entity was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date and time when the entity was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Indicates whether the entity is active (for soft delete functionality)
    /// </summary>
    public bool IsActive { get; set; } = true;
}