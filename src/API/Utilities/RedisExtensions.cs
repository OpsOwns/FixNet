namespace API.Utilities;

public static class RedisExtensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnection = configuration["Redis:ConnectionString"]
                              ?? throw new InvalidOperationException("Redis connection string is missing");

        var redisInstance = configuration["Redis:Instance"] ?? throw new InvalidOperationException("Redis instance is missing");

        services.AddHealthChecks()
            .AddRedis(redisConnection, name: "redis");

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
            options.InstanceName = $"{redisInstance}:";
        });

        return services;
    }
}