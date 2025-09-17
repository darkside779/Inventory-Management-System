using System.ComponentModel.DataAnnotations;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Application.DTOs;

/// <summary>
/// Data Transfer Object for User entity
/// </summary>
public class UserDto
{
    /// <summary>
    /// Unique identifier for the user
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Username of the user
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Email address of the user
    /// </summary>
    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// First name of the user
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Last name of the user
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Full name of the user
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";
    
    /// <summary>
    /// Role of the user
    /// </summary>
    public UserRole Role { get; set; }
    
    /// <summary>
    /// Indicates whether the user is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Date and time when the user was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Date and time when the user was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Date and time of last login
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
}

/// <summary>
/// DTO for creating a new user
/// </summary>
public class CreateUserDto
{
    /// <summary>
    /// Username of the user
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Email address of the user
    /// </summary>
    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Password for the user
    /// </summary>
    [Required]
    [MinLength(6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// First name of the user
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Last name of the user
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Role of the user
    /// </summary>
    [Required]
    public UserRole Role { get; set; }
    
    /// <summary>
    /// Indicates whether the user is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO for updating an existing user
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// Unique identifier for the user
    /// </summary>
    [Required]
    public int Id { get; set; }
    
    /// <summary>
    /// Username of the user
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Email address of the user
    /// </summary>
    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// First name of the user
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Last name of the user
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Role of the user
    /// </summary>
    [Required]
    public UserRole Role { get; set; }
    
    /// <summary>
    /// Indicates whether the user is active
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for changing user password
/// </summary>
public class ChangePasswordDto
{
    /// <summary>
    /// Unique identifier for the user
    /// </summary>
    [Required]
    public int Id { get; set; }
    
    /// <summary>
    /// Current password
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;
    
    /// <summary>
    /// New password
    /// </summary>
    [Required]
    [MinLength(6)]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;
    
    /// <summary>
    /// Confirm new password
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// Simplified DTO for user lookups and dropdowns
/// </summary>
public class UserLookupDto
{
    /// <summary>
    /// Unique identifier for the user
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Username of the user
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Full name of the user
    /// </summary>
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// Role of the user
    /// </summary>
    public UserRole Role { get; set; }
    
    /// <summary>
    /// Indicates whether the user is active
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for user login
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Username or email for login
    /// </summary>
    [Required]
    public string UsernameOrEmail { get; set; } = string.Empty;
    
    /// <summary>
    /// Password for login
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// Remember me option
    /// </summary>
    public bool RememberMe { get; set; }
}
