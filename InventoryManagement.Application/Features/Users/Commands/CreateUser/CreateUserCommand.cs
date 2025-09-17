using MediatR;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Features.Users.Commands.CreateUser;

/// <summary>
/// Command to create a new user
/// </summary>
public class CreateUserCommand : IRequest<UserDto>
{
    /// <summary>
    /// Username of the user
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Email address of the user
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Password for the user
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// Full name of the user
    /// </summary>
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// Phone number of the user
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Role of the user
    /// </summary>
    public Domain.Enums.UserRole Role { get; set; }
    
    /// <summary>
    /// Whether the user is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Whether to send welcome email
    /// </summary>
    public bool SendWelcomeEmail { get; set; } = true;
}
