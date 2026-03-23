using Microsoft.Extensions.Caching.Hybrid;

namespace FixNet.API.Utilities;

public static class CachingExtensions
{
    public static IServiceCollection AddFixNetCaching(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnection = configuration["Redis:ConnectionString"]
                              ?? throw new InvalidOperationException("Redis connection string is missing");
        var redisInstance = configuration["Redis:Instance"]
                            ?? throw new InvalidOperationException("Redis instance is missing");
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
            options.InstanceName = $"{redisInstance}:";
        });

        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(60),
                LocalCacheExpiration = TimeSpan.FromSeconds(30)
            };
        });

        services.AddHealthChecks()
            .AddRedis(redisConnection, name: "redis");

        return services;
    }
}