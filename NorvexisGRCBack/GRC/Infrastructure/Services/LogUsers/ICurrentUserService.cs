namespace GRC.Infrastructure.Services;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Email { get; }
    string? UserName { get; }
}