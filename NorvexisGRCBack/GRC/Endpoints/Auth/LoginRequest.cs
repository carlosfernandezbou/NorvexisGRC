namespace GRC.Endpoints.Auth;

public record LoginRequest(
    string Email,
    string Password
);