using System.ComponentModel.DataAnnotations;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Domain.Entities;

/// <summary>
/// Represents a system user
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Unique username for login
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Email address
    /// </summary>
    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Hashed password
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>
    /// User's full name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// User's role in the system
    /// </summary>
    [Required]
    public UserRole Role { get; set; }
    
    /// <summary>
    /// Phone number
    /// </summary>
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Last login timestamp
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
    
    /// <summary>
    /// Indicates whether the user account is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    
    /// <summary>
    /// Transactions created by this user
    /// </summary>
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}