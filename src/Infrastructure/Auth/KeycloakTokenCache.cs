namespace FixNet.Infrastructure.Auth;

public sealed class KeycloakTokenCache(TimeProvider timeProvider)
{
    private CachedToken? _cache;

    public string? GetToken()
    {
        var current = _cache;

        if (current is null)
            return null;

        if (timeProvider.GetUtcNow() >= current.ExpiresAt)
            return null;

        return current.AccessToken;
    }

    public void SetToken(string token, int expiresInSeconds)
    {
        var expiresAt = timeProvider.GetUtcNow()
            .AddSeconds(expiresInSeconds - 30);

        var newCache = new CachedToken(token, expiresAt);

        _cache = newCache;
    }

    private sealed record CachedToken(string AccessToken, DateTimeOffset ExpiresAt);
}