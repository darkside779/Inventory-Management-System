using MediatR;

namespace InventoryManagement.Application.Features.Users.Commands.ChangePassword;

/// <summary>
/// Command to change user password
/// </summary>
public class ChangePasswordCommand : IRequest<bool>
{
    /// <summary>
    /// User ID
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// Current password (required when user changes own password)
    /// </summary>
    public string? CurrentPassword { get; set; }
    
    /// <summary>
    /// New password
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether this is an admin reset (bypasses current password check)
    /// </summary>
    public bool IsAdminReset { get; set; } = false;
    
    /// <summary>
    /// ID of user performing the action (for logging)
    /// </summary>
    public int? ChangedByUserId { get; set; }
}
