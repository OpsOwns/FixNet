using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FixNet.Infrastructure.Auth;

internal sealed class CachedKeycloakTokenProvider(
    IHttpClientFactory httpClientFactory,
    IOptions<KeycloakSettings> settings,
    TimeProvider timeProvider,
    ILogger<CachedKeycloakTokenProvider> logger) : IDisposable
{
    private readonly KeycloakSettings _settings = settings.Value;
    private string? _cachedToken;
    private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task<string> GetTokenAsync(CancellationToken ct)

    {
        if (_cachedToken is not null && timeProvider.GetUtcNow() < _expiresAt)
            return _cachedToken;

        await _semaphore.WaitAsync(ct);

        try
        {
            if (_cachedToken is not null && timeProvider.GetUtcNow() < _expiresAt)
                return _cachedToken;

            logger.LogInformation("Refreshing Keycloak admin token for realm: {Realm}", _settings.Realm);

            var httpClient = httpClientFactory.CreateClient(nameof(CachedKeycloakTokenProvider));

            var response = await httpClient.PostAsync(
                $"{_settings.BaseUrl}/realms/{_settings.Realm}" +
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

            _cachedToken = tokenResponse.AccessToken;
            _expiresAt = timeProvider.GetUtcNow().AddSeconds(tokenResponse.ExpiresIn - 30);

            return _cachedToken;
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