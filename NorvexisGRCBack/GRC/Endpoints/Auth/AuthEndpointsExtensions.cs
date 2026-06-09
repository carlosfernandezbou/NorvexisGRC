using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GRC.Core.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Audit.Core;

namespace GRC.Endpoints.Auth;

public static class AuthEndpointsExtensions
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/api/auth/login", async (
            LoginRequest request,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration) =>
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is null || !user.IsActive)
            {
                await Task.Delay(500);

                await using (var audit = await AuditScope.CreateAsync("User:LoginFailed", () => new
                {
                    Email = request.Email,
                    Reason = user is null ? "UserNotFound" : "UserInactive",
                    Action = "Login",
                    Success = false,
                    TimestampUtc = DateTime.UtcNow
                }))
                    return Results.Unauthorized();
            }

            var result = await signInManager.CheckPasswordSignInAsync(
                user,
                request.Password,
                lockoutOnFailure: true
            );

            if (result.IsLockedOut)
            {
                await using (var audit = await AuditScope.CreateAsync("User:LockedOut", () => new
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Action = "Login",
                    Success = false,
                    Reason = "LockedOut",
                    TimestampUtc = DateTime.UtcNow
                }))
                {
                    return Results.StatusCode(423);
                }
            }

            if (!result.Succeeded)
            {
                await using (var audit = await AuditScope.CreateAsync("User:LoginFailed", () => new
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Reason = "InvalidPassword",
                    Action = "Login",
                    Success = false,
                    TimestampUtc = DateTime.UtcNow
                }))
                {
                    return Results.Unauthorized();
                }
            }

            var roles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? "")
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Missing config: Jwt:Key");

            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];
            var expiresMinutes = int.Parse(configuration["Jwt:ExpiresMinutes"] ?? "60");

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            await using (var audit = await AuditScope.CreateAsync("User:Login", () => new
            {
                UserId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Name = user.Name,
                LastName = user.LastName,
                Roles = roles,
                Action = "Login",
                Success = true,
                TimestampUtc = DateTime.UtcNow
            }))

                return Results.Ok(new
                {
                    accessToken = tokenString,
                    expiresInMinutes = expiresMinutes,
                    user = new
                    {
                        user.Id,
                        user.Email,
                        user.Name,
                        user.LastName,
                        roles
                    }
                });
        })
        .WithTags("Auth");

        app.MapPost("/api/auth/logout", async (
            ClaimsPrincipal principal) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = principal.FindFirstValue(ClaimTypes.Email);
            var userName = principal.Identity?.Name;

            await using (var audit = await AuditScope.CreateAsync("User:Logout", () => new
            {
                UserId = userId,
                Email = email,
                UserName = userName,
                Action = "Logout",
                Success = true,
                TimestampUtc = DateTime.UtcNow
            }))

                return Results.Ok(new
                {
                    message = "Logout successful"
                });
        })
        .RequireAuthorization()
        .WithTags("Auth");

    }
}