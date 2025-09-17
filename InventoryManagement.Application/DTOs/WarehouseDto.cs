using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs;

/// <summary>
/// Data Transfer Object for Warehouse entity
/// </summary>
public class WarehouseDto
{
    /// <summary>
    /// Unique identifier for the warehouse
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Name of the warehouse
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Location/address of the warehouse
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the warehouse
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Maximum capacity of the warehouse
    /// </summary>
    public int? Capacity { get; set; }
    
    /// <summary>
    /// Indicates whether the warehouse is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Date and time when the warehouse was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Date and time when the warehouse was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Current inventory item count in this warehouse
    /// </summary>
    public int InventoryItemCount { get; set; }
    
    /// <summary>
    /// Current capacity utilization percentage
    /// </summary>
    public decimal? CapacityUtilization { get; set; }
}

/// <summary>
/// DTO for creating a new warehouse
/// </summary>
public class CreateWarehouseDto
{
    /// <summary>
    /// Name of the warehouse
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Location/address of the warehouse
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the warehouse
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Maximum capacity of the warehouse
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than zero")]
    public int? Capacity { get; set; }
    
    /// <summary>
    /// Indicates whether the warehouse is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO for updating an existing warehouse
/// </summary>
public class UpdateWarehouseDto
{
    /// <summary>
    /// Unique identifier for the warehouse
    /// </summary>
    [Required]
    public int Id { get; set; }
    
    /// <summary>
    /// Name of the warehouse
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Location/address of the warehouse
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the warehouse
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Maximum capacity of the warehouse
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than zero")]
    public int? Capacity { get; set; }
    
    /// <summary>
    /// Indicates whether the warehouse is active
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// Simplified DTO for warehouse lookups and dropdowns
/// </summary>
public class WarehouseLookupDto
{
    /// <summary>
    /// Unique identifier for the warehouse
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Name of the warehouse
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Location of the warehouse
    /// </summary>
    public string Location { get; set; } = string.Empty;
    
    /// <summary>
    /// Indicates whether the warehouse is active
    /// </summary>
    public bool IsActive { get; set; }
}
