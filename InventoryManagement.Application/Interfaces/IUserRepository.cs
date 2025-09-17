using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Application.Interfaces;

/// <summary>
/// Repository interface for User entity with specific business operations
/// </summary>
public interface IUserRepository : IGenericRepository<User>
{
    /// <summary>
    /// Get user by username
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User or null</returns>
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user by email
    /// </summary>
    /// <param name="email">Email address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User or null</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get users by role
    /// </summary>
    /// <param name="role">User role</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Users with the specified role</returns>
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if username is unique
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="excludeUserId">User ID to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if username is unique</returns>
    Task<bool> IsUsernameUniqueAsync(string username, int? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if email is unique
    /// </summary>
    /// <param name="email">Email address</param>
    /// <param name="excludeUserId">User ID to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if email is unique</returns>
    Task<bool> IsEmailUniqueAsync(string email, int? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active users only
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Active users</returns>
    Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Update last login time
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="loginTime">Login time</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if updated successfully</returns>
    Task<bool> UpdateLastLoginAsync(int userId, DateTime loginTime, CancellationToken cancellationToken = default);
}
