using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FixNet.Infrastructure.Auth;

internal sealed class KeycloakTokenProvider(
    HttpClient httpClient,
    IOptions<KeycloakSettings> settings,
    ILogger<KeycloakTokenProvider> logger,
    KeycloakTokenCache cache) : IDisposable
{
    private readonly KeycloakSettings _settings = settings.Value;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task<string> GetTokenAsync(CancellationToken ct)
    {
        var cached = cache.GetToken();

        if (cached is not null)
            return cached;

        await _semaphore.WaitAsync(ct);

        try
        {
            if (cached is not null)
                return cached;

            logger.LogInformation("Refreshing Keycloak admin token for realm: {Realm}", _settings.Realm);

            var response = await httpClient.PostAsync(
                $"/realms/{_settings.Realm}" +
                "/protocol/openid-connect/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "client_credentials",
                    ["client_id"] = _settings.AdminClientId,
                    ["client_secret"] = _settings.AdminClientSecret
                }), ct);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);

                throw new IdentityProviderException(
                    $"Failed to get admin token: {response.StatusCode}. {body}",
                    IdentityProviderErrorCode.Unauthorized);
            }

            var tokenResponse = await response.Content
                .ReadFromJsonAsync<KeycloakTokenResponse>(ct) ?? throw new IdentityProviderException(
                "Keycloak token endpoint returned empty or invalid response.",
                IdentityProviderErrorCode.Unavailable);

            cache.SetToken(tokenResponse.AccessToken, tokenResponse.ExpiresIn);
            return tokenResponse.AccessToken;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        _semaphore.Dispose();
    }
}