using Microsoft.AspNetCore.Identity;
using ReportHub.Identity.Models;

namespace ReportHub.Identity.Workers;

public class DatabaseSeeder : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseSeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        await SeedRoles(scope);
        await SeedUsers(scope);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // No cleanup required
        return Task.CompletedTask;
    }

    private async Task SeedRoles(AsyncServiceScope scope)
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        var roles = new List<string>
        {
            "SystemAdmin",
            "Admin",
            "User"
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role(role));
            }
        }
    }

    private async Task SeedUsers(AsyncServiceScope scope)
    {
        const string adminEmail = "admin@example.com";
        const string adminPassword = "Admin123$";
        const string adminRole = "Admin";

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = await userManager.FindByEmailAsync(adminEmail);
        if (user is null)
        {
            user = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, adminRole);
            }
        }
    }
}