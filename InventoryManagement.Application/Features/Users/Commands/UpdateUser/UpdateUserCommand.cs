using MediatR;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Features.Users.Commands.UpdateUser;

/// <summary>
/// Command to update an existing user
/// </summary>
public class UpdateUserCommand : IRequest<UserDto>
{
    /// <summary>
    /// User ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Username of the user
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Email address of the user
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
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
    public bool IsActive { get; set; }
}
