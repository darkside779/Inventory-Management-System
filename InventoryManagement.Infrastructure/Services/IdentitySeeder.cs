using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Infrastructure.Services;

/// <summary>
/// Seeds initial Identity data including roles and admin user
/// </summary>
public static class IdentitySeeder
{
    /// <summary>
    /// Seeds roles and creates initial admin user
    /// </summary>
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("IdentitySeeder");

        try
        {
            // Define roles
            string[] roles = { "Administrator", "Manager", "Staff" };

            // Create roles if they don't exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (result.Succeeded)
                    {
                        logger.LogInformation("Created role: {Role}", role);
                    }
                    else
                    {
                        logger.LogError("Failed to create role {Role}: {Errors}", role, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }

            // Seed users and roles
            var usersToSeed = new[]
            {
                new { Email = "admin@inventoryms.com", Password = "Admin@123", Role = "Administrator" },
                new { Email = "manager1@inventoryms.com", Password = "Admin@123", Role = "Manager" },
                new { Email = "manager2@inventoryms.com", Password = "Admin@123", Role = "Manager" },
                new { Email = "staff1@inventoryms.com", Password = "Admin@123", Role = "Staff" },
                new { Email = "staff2@inventoryms.com", Password = "Admin@123", Role = "Staff" }
            };

            foreach (var userSeed in usersToSeed)
            {
                var user = await userManager.FindByEmailAsync(userSeed.Email);
                if (user == null)
                {
                    user = new IdentityUser
                    {
                        UserName = userSeed.Email,
                        Email = userSeed.Email,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(user, userSeed.Password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, userSeed.Role);
                        logger.LogInformation($"Created user: {userSeed.Email} with role: {userSeed.Role}");
                    }
                    else
                    {
                        logger.LogError($"Failed to create user {userSeed.Email}: {{Errors}}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    if (!await userManager.IsInRoleAsync(user, userSeed.Role))
                    {
                        await userManager.AddToRoleAsync(user, userSeed.Role);
                        logger.LogInformation($"Added role {userSeed.Role} to existing user: {userSeed.Email}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding Identity data");
        }
    }
}
