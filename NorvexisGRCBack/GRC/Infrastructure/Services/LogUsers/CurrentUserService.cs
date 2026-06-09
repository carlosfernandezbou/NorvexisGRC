using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace GRC.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId =>
        _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier)
        ??
        _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue("sub");

    public string? Email =>
        _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.Email);

    public string? UserName =>
        _httpContextAccessor.HttpContext?
            .User
            .Identity?
            .Name;
}