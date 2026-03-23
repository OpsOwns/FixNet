using System.Text.Json.Serialization;

namespace FixNet.Infrastructure.Auth;

internal class KeycloakTokenResponse

{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
}