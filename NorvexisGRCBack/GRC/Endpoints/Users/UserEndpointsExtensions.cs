using GRC.Core.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GRC.Endpoints.Users;

public static class UserEndpointsExtensions
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapGet("/api/users", async (UserManager<User> UserManager) =>
        {
            var users = await UserManager.Users
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.Name,
                    u.LastName,
                    u.IsActive
                })
                .ToListAsync();

            return Results.Ok(users);
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        app.MapGet("/api/users/{id}", async (string id, UserManager<User> UserManager) =>
        {
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
                return Results.NotFound();

            var roles = await UserManager.GetRolesAsync(user);

            return Results.Ok(new
            {
                user.Id,
                user.Email,
                user.Name,
                user.LastName,
                user.IsActive,
                roles
            });
        })
        .RequireAuthorization();

        app.MapPost("/api/users", async (
            CreateUserRequest request,
            UserManager<User> UserManager) =>
        {
            var user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                Name = request.Name,
                LastName = request.LastName,
                IsActive = true
            };

            var result = await UserManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return Results.BadRequest(result.Errors);

            return Results.Ok(new
            {
                user.Id,
                user.Email,
                user.Name,
                user.LastName,
                user.IsActive
            });
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        app.MapPut("/api/users/{id}", async (
            string id,
            UpdateUserRequest request,
            UserManager<User> UserManager) =>
        {
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
                return Results.NotFound();

            user.Email = request.Email;
            user.UserName = request.Email;
            user.Name = request.Name;
            user.LastName = request.LastName;

            var result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded)
                return Results.BadRequest(result.Errors);

            return Results.Ok(new
            {
                user.Id,
                user.Email,
                user.Name,
                user.LastName,
                user.IsActive
            });
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        app.MapDelete("/api/users/{id}", async (
            string id,
            UserManager<User> userManager) =>
        {
            var user = await userManager.FindByIdAsync(id);

            if (user is null)
                return Results.NotFound($"User with id {id} not found");

            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return Results.BadRequest(result.Errors);

            return Results.NoContent();
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        // ROLES
        app.MapGet("/api/users/roles", async (RoleManager<IdentityRole> roleManager) =>
        {
            var roles = await roleManager.Roles
                .Select(r => new
                {
                    r.Id,
                    r.Name
                })
                .ToListAsync();

            return Results.Ok(roles);
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));

        app.MapPost("/api/users/roles", async (
            string roleName,
            RoleManager<IdentityRole> roleManager) =>
        {
            var exists = await roleManager.RoleExistsAsync(roleName);

            if (exists)
                return Results.BadRequest("Role already exists");

            var result = await roleManager.CreateAsync(new IdentityRole(roleName));

            if (!result.Succeeded)
                return Results.BadRequest(result.Errors);

            return Results.Ok(new { role = roleName });
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));

        app.MapGet("/api/users/{id}/roles", async (
            string id,
            UserManager<User> userManager) =>
        {
            var user = await userManager.FindByIdAsync(id);

            if (user is null)
                return Results.NotFound();

            var roles = await userManager.GetRolesAsync(user);

            return Results.Ok(roles);
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        app.MapPost("/api/users/{id}/roles/{role}", async (
            string id,
            string role,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager) =>
        {
            var user = await userManager.FindByIdAsync(id);

            if (user is null)
                return Results.NotFound();

            var roleExists = await roleManager.RoleExistsAsync(role);

            if (!roleExists)
                return Results.NotFound($"Role '{role}' does not exist");

            var alreadyInRole = await userManager.IsInRoleAsync(user, role);

            if (alreadyInRole)
                return Results.BadRequest($"User already has role '{role}'");

            var result = await userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
                return Results.BadRequest(result.Errors);

            return Results.Ok();
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));

        app.MapDelete("/api/users/{id}/roles/{role}", async (
            string id,
            string role,
            UserManager<User> userManager) =>
        {
            var user = await userManager.FindByIdAsync(id);

            if (user is null)
                return Results.NotFound();

            var result = await userManager.RemoveFromRoleAsync(user, role);

            if (!result.Succeeded)
                return Results.BadRequest(result.Errors);

            return Results.Ok();
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }
}


