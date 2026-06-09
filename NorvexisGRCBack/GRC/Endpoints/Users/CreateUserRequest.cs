namespace GRC.Endpoints.Users;

public record CreateUserRequest(
    string Email,
    string Password,
    string Name,
    string LastName
);