namespace FixNet.Infrastructure.Persistence.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsAvailable { get; set; }
}