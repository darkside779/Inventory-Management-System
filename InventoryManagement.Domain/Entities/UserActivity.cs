using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Domain.Entities;

/// <summary>
/// Represents a user activity log entry
/// </summary>
public class UserActivity : BaseEntity
{
    /// <summary>
    /// ID of the user who performed the activity
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Type of activity performed
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ActivityType { get; set; } = string.Empty;

    /// <summary>
    /// Description of the activity
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// IP address from which the activity was performed
    /// </summary>
    [MaxLength(45)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent string
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    public string? Metadata { get; set; }

    // Navigation properties

    /// <summary>
    /// User who performed the activity
    /// </summary>
    public virtual User User { get; set; } = null!;
}
