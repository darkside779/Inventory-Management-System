using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Domain.Entities;

/// <summary>
/// Represents a supplier or vendor
/// </summary>
public class Supplier : BaseEntity
{
    /// <summary>
    /// Name of the supplier company
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Primary contact information
    /// </summary>
    [MaxLength(200)]
    public string? ContactInfo { get; set; }
    
    /// <summary>
    /// Supplier's address
    /// </summary>
    [MaxLength(500)]
    public string? Address { get; set; }
    
    /// <summary>
    /// Phone number
    /// </summary>
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    /// <summary>
    /// Email address
    /// </summary>
    [MaxLength(100)]
    [EmailAddress]
    public string? Email { get; set; }
    
    /// <summary>
    /// Company website URL
    /// </summary>
    [MaxLength(200)]
    [Url]
    public string? Website { get; set; }
    
    /// <summary>
    /// Tax identification number
    /// </summary>
    [MaxLength(50)]
    public string? TaxNumber { get; set; }
    
    /// <summary>
    /// Payment terms (e.g., "Net 30")
    /// </summary>
    [MaxLength(100)]
    public string? PaymentTerms { get; set; }
    
    /// <summary>
    /// Indicates whether the supplier is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    
    /// <summary>
    /// Products supplied by this supplier
    /// </summary>
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}