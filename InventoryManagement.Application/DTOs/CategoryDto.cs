using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs;

/// <summary>
/// Data Transfer Object for Category entity
/// </summary>
public class CategoryDto
{
    /// <summary>
    /// Unique identifier for the category
    /// </summary>
    public int Id { get; set; }
    
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
    
    /// <summary>
    /// Date and time when the category was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Date and time when the category was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Number of products in this category
    /// </summary>
    public int ProductCount { get; set; }
}

/// <summary>
/// DTO for creating a new category
/// </summary>
public class CreateCategoryDto
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
}

/// <summary>
/// DTO for updating an existing category
/// </summary>
public class UpdateCategoryDto
{
    /// <summary>
    /// Unique identifier for the category
    /// </summary>
    [Required]
    public int Id { get; set; }
    
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
    public bool IsActive { get; set; }
}

/// <summary>
/// Simplified DTO for category lookups and dropdowns
/// </summary>
public class CategoryLookupDto
{
    /// <summary>
    /// Unique identifier for the category
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Name of the category
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Indicates whether the category is active
    /// </summary>
    public bool IsActive { get; set; }
}
