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

            // Create default admin user if it doesn't exist
            const string adminEmail = "admin@inventorymanagement.com";
            const string adminPassword = "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    // Add admin to Administrator role
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                    logger.LogInformation("Created default admin user: {Email}", adminEmail);
                }
                else
                {
                    logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                // Ensure existing admin user has Administrator role
                if (!await userManager.IsInRoleAsync(adminUser, "Administrator"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                    logger.LogInformation("Added Administrator role to existing user: {Email}", adminEmail);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding Identity data");
        }
    }
}
