using System.Text.Json.Serialization;

namespace FixNet.Infrastructure.Auth.Keycloak;

public class KeycloakTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
}