namespace FixNet.Application.Users;

public record CreateIdentityRequest(
    string Email,
    string FirstName,
    string LastName,
    string Password);