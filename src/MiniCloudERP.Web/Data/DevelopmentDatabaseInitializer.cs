using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniCloudERP.DataAccess.Identity;
using MiniCloudERP.DataAccess.Persistence;

namespace MiniCloudERP.Web.Data;

/// <summary>
/// Applies pending EF Core migrations and seeds the baseline Identity roles and
/// two demo accounts (Admin/User) so the app is usable immediately after
/// cloning the repository and pointing it at a fresh SQL Server database.
/// </summary>
public static class DevelopmentDatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        var dbContext = provider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();

        await SeedRolesAsync(provider);
        await SeedUsersAsync(provider);
    }

    private static async Task SeedRolesAsync(IServiceProvider provider)
    {
        var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var role in AppRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedUsersAsync(IServiceProvider provider)
    {
        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();

        await EnsureUserAsync(userManager, "admin@minicloud.local", "Admin123!", "System Administrator", AppRoles.Admin);
        await EnsureUserAsync(userManager, "user@minicloud.local", "User123!", "Demo User", AppRoles.User);
    }

    private static async Task EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string fullName,
        string role)
    {
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is not null)
        {
            return;
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FullName = fullName
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}
