namespace GRC.Endpoints.Users;

public record UpdateUserRequest(
    string Email,
    string Password,
    string Name,
    string LastName
);