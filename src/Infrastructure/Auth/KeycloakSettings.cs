namespace FixNet.Infrastructure.Auth;

public class KeycloakSettings
{
    public const string SectionName = "Keycloak";
    public string BaseUrl { get; set; } = string.Empty;
    public string Realm { get; set; } = string.Empty;
    public string AdminClientId { get; set; } = string.Empty;
    public string AdminClientSecret { get; set; } = string.Empty;
}