using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Infrastructure.Services;

/// <summary>
/// Implementation of IIdentityService for ASP.NET Core Identity operations
/// </summary>
public class IdentityService : IIdentityService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<IdentityService> _logger;

    public IdentityService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<IdentityService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<(bool Success, string? UserId, string? Error)> CreateUserAsync(string email, string password, UserRole role, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating Identity user for email: {Email}", email);

            // Create new Identity user
            var identityUser = new IdentityUser
            {
                UserName = email, // Use email as username for consistency
                Email = email,
                EmailConfirmed = true // Auto-confirm for admin-created users
            };

            var identityResult = await _userManager.CreateAsync(identityUser, password);
            if (!identityResult.Succeeded)
            {
                var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                _logger.LogError("Failed to create Identity user: {Errors}", errors);
                return (false, null, errors);
            }

            // Add user to appropriate role in Identity system
            var roleName = role.ToString();
            
            // Ensure role exists
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                _logger.LogWarning("Role {Role} does not exist, creating it", roleName);
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (!roleResult.Succeeded)
                {
                    _logger.LogError("Failed to create role {Role}", roleName);
                }
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(identityUser, roleName);
            if (!addToRoleResult.Succeeded)
            {
                _logger.LogWarning("Failed to add user to role {Role}. User created but without role.", roleName);
            }

            _logger.LogInformation("Identity user created successfully with ID: {UserId}", identityUser.Id);
            return (true, identityUser.Id, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Identity user for email: {Email}", email);
            return (false, null, ex.Message);
        }
    }

    public async Task<bool> UserExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user exists by email: {Email}", email);
            return false;
        }
    }

    public async Task<bool> DeleteUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found for deletion", email);
                return false;
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("Identity user deleted successfully for email: {Email}", email);
                return true;
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogError("Failed to delete Identity user: {Errors}", errors);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting Identity user for email: {Email}", email);
            return false;
        }
    }
}
