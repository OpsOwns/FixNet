namespace FixNet.Application;

public record ExternalIdentity(string Email, string Password, string FirstName, string LastName, string? Phone = null);