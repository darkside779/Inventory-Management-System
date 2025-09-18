using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Application.Interfaces;

/// <summary>
/// Interface for Identity operations
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Create a new Identity user
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="password">User password</param>
    /// <param name="role">User role</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status and user ID if successful</returns>
    Task<(bool Success, string? UserId, string? Error)> CreateUserAsync(string email, string password, UserRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a user exists by email
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user exists</returns>
    Task<bool> UserExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a user by email
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    Task<bool> DeleteUserByEmailAsync(string email, CancellationToken cancellationToken = default);
}
