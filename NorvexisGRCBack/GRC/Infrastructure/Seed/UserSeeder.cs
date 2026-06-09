using System.Text.Json;
using GRC.Core.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GRC.Infrastructure.Data.Seed;

public static class UserSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<IdentityCosmosDbContext>();

        await SeedRoles(context);
        await SeedUsers(context);
        await SeedUserRoles(context);

        await context.SaveChangesAsync();
    }

    private static async Task SeedRoles(IdentityCosmosDbContext context)
    {
        var roles = await ReadJsonAsync<List<IdentityRole>>("Identity_Roles.json");

        foreach (var role in roles)
        {
            var existingRole = await context.Roles
                .FirstOrDefaultAsync(x => x.Id == role.Id);

            if (existingRole is null)
            {
                context.Roles.Add(role);
            }
        }
    }

    private static async Task SeedUsers(IdentityCosmosDbContext context)
    {
        var users = await ReadJsonAsync<List<User>>("Identity.json");

        foreach (var user in users)
        {
            var existingUser = await context.Users
                .FirstOrDefaultAsync(x => x.Id == user.Id);

            if (existingUser is null)
            {
                context.Users.Add(user);
            }
        }
    }

    private static async Task SeedUserRoles(IdentityCosmosDbContext context)
    {
        var userRoles = await ReadJsonAsync<List<IdentityUserRole<string>>>("Identity_UserRoles.json");

        foreach (var userRole in userRoles)
        {
            var existingUserRole = await context.UserRoles
                .FirstOrDefaultAsync(x =>
                    x.UserId == userRole.UserId &&
                    x.RoleId == userRole.RoleId
                );

            if (existingUserRole is null)
            {
                context.UserRoles.Add(userRole);
            }
        }
    }

    private static async Task<T> ReadJsonAsync<T>(string fileName)
    {
        var fullPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Infrastructure",
            "Seed",
            "Json",
            fileName
        );

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Seed file not found: {fullPath}");

        var json = await File.ReadAllTextAsync(fullPath);

        return JsonSerializer.Deserialize<T>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        )!;
    }
    
}