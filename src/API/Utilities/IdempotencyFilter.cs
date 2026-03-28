using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Hybrid;

namespace FixNet.API.Utilities;

internal sealed class IdempotencyFilter(int cacheTimeInMinutes = 60, ILogger<IdempotencyFilter>? logger = null)
    : IEndpointFilter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var request = context.HttpContext.Request;

        if (!request.Headers.TryGetValue("X-Idempotency-Key", out var idempotencyKey) || string.IsNullOrWhiteSpace(idempotencyKey))
        {
            return Results.BadRequest("Missing X-Idempotency-Key header");
        }

        var key = idempotencyKey.ToString();

        if (key.Length > 100)
        {
            return Results.BadRequest("X-Idempotency-Key too long");
        }

        var bodyHash = await ComputeBodyHashAsync(request);

        var cache = context.HttpContext.RequestServices.GetRequiredService<HybridCache>();
        var userId = context.HttpContext.User.Identity?.Name ?? "anon";
        var cacheKey = $"idempotency:{userId}:{request.Method}:{request.Path}:{bodyHash}:{key}";

        var ct = context.HttpContext.RequestAborted;

        var cached = await cache.GetOrCreateAsync(
            cacheKey,
            async _ =>
            {
                var result = await next(context);

                if (ct.IsCancellationRequested)
                {
                    logger?.LogIdempotencyCancelled(key);
                    return null;
                }

                if (result is not (IStatusCodeHttpResult scr and IValueHttpResult vr) ||
                    scr.StatusCode is null or < 200 or >= 300)
                {
                    return null;
                }

                var status = scr.StatusCode ?? StatusCodes.Status200OK;
                var json = JsonSerializer.Serialize(vr.Value, JsonOptions);

                return new IdempotentResponse(status, json);
            },
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(cacheTimeInMinutes)
            },
            cancellationToken: ct
        );

        if (cached is null)
        {
            return await next(context);
        }

        logger?.LogIdempotencyHit(key);
        return new IdempotentResult(cached.StatusCode, cached.Json);
    }

    private static async Task<string> ComputeBodyHashAsync(HttpRequest request)
    {
        if (request.ContentLength is null or 0)
        {
            return "none";
        }

        request.EnableBuffering();
        request.Body.Position = 0;
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashBytes = await sha256.ComputeHashAsync(request.Body);
        request.Body.Position = 0;

        return Convert.ToHexString(hashBytes);
    }

    private sealed class IdempotentResult(int statusCode, string json) : IResult
    {
        public async Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.Headers.CacheControl = "no-store";
            httpContext.Response.Headers["X-Cache-Hit"] = "true";
            await httpContext.Response.WriteAsync(json);
        }
    }

    private sealed record IdempotentResponse(int StatusCode, string Json);
}