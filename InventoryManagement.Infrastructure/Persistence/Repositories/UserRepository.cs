using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// User repository implementation with specific business operations
/// </summary>
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get user by username
    /// </summary>
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    /// <summary>
    /// Get user by email
    /// </summary>
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    /// <summary>
    /// Get users by role
    /// </summary>
    public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(u => u.Role == role && u.IsActive)
            .OrderBy(u => u.FullName)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Check if username is unique
    /// </summary>
    public async Task<bool> IsUsernameUniqueAsync(string username, int? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(u => u.Username == username);
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Check if email is unique
    /// </summary>
    public async Task<bool> IsEmailUniqueAsync(string email, int? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(u => u.Email == email);
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Get active users only
    /// </summary>
    public async Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(u => u.IsActive)
            .OrderBy(u => u.FullName)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Update last login time
    /// </summary>
    public async Task<bool> UpdateLastLoginAsync(int userId, DateTime loginTime, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return false;
        }

        user.LastLoginAt = loginTime;
        Update(user);
        return true;
    }
}
