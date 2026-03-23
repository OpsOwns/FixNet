namespace FixNet.Application.Abstractions;

public record CreateIdentityRequest(
    string Email,
    string FirstName,
    string LastName,
    string Password);