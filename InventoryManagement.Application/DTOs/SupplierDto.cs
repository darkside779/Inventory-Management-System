using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs;

/// <summary>
/// Data Transfer Object for Supplier entity
/// </summary>
public class SupplierDto
{
    /// <summary>
    /// Unique identifier for the supplier
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Name of the supplier
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Contact person at the supplier
    /// </summary>
    [MaxLength(100)]
    public string? ContactPerson { get; set; }
    
    /// <summary>
    /// Phone number of the supplier
    /// </summary>
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    /// <summary>
    /// Email address of the supplier
    /// </summary>
    [MaxLength(100)]
    [EmailAddress]
    public string? Email { get; set; }
    
    /// <summary>
    /// Address of the supplier
    /// </summary>
    [MaxLength(200)]
    public string? Address { get; set; }
    
    /// <summary>
    /// Additional notes about the supplier
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    /// <summary>
    /// Indicates whether the supplier is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Date and time when the supplier was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Date and time when the supplier was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Number of products supplied by this supplier
    /// </summary>
    public int ProductCount { get; set; }
}

/// <summary>
/// DTO for creating a new supplier
/// </summary>
public class CreateSupplierDto
{
    /// <summary>
    /// Name of the supplier
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Contact person at the supplier
    /// </summary>
    [MaxLength(100)]
    public string? ContactPerson { get; set; }
    
    /// <summary>
    /// Phone number of the supplier
    /// </summary>
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    /// <summary>
    /// Email address of the supplier
    /// </summary>
    [MaxLength(100)]
    [EmailAddress]
    public string? Email { get; set; }
    
    /// <summary>
    /// Address of the supplier
    /// </summary>
    [MaxLength(200)]
    public string? Address { get; set; }
    
    /// <summary>
    /// Additional notes about the supplier
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    /// <summary>
    /// Indicates whether the supplier is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO for updating an existing supplier
/// </summary>
public class UpdateSupplierDto
{
    /// <summary>
    /// Unique identifier for the supplier
    /// </summary>
    [Required]
    public int Id { get; set; }
    
    /// <summary>
    /// Name of the supplier
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Contact person at the supplier
    /// </summary>
    [MaxLength(100)]
    public string? ContactPerson { get; set; }
    
    /// <summary>
    /// Phone number of the supplier
    /// </summary>
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    /// <summary>
    /// Email address of the supplier
    /// </summary>
    [MaxLength(100)]
    [EmailAddress]
    public string? Email { get; set; }
    
    /// <summary>
    /// Address of the supplier
    /// </summary>
    [MaxLength(200)]
    public string? Address { get; set; }
    
    /// <summary>
    /// Additional notes about the supplier
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    /// <summary>
    /// Indicates whether the supplier is active
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// Simplified DTO for supplier lookups and dropdowns
/// </summary>
public class SupplierLookupDto
{
    /// <summary>
    /// Unique identifier for the supplier
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Name of the supplier
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Contact person at the supplier
    /// </summary>
    public string? ContactPerson { get; set; }
    
    /// <summary>
    /// Indicates whether the supplier is active
    /// </summary>
    public bool IsActive { get; set; }
}
