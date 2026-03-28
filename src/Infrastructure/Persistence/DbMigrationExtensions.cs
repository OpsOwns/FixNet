using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace FixNet.Infrastructure.Persistence;

public static class DbMigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FixNetDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<FixNetDbContext>>();

        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(3),
                BackoffType = DelayBackoffType.Constant,
                OnRetry = args =>
                {
                    logger.LogWarning("Migration attempt {Attempt} failed. Retrying in {Delay}s. Error: {Message}",
                        args.AttemptNumber + 1,
                        args.RetryDelay.TotalSeconds,
                        args.Outcome.Exception?.Message);
                    return default;
                }
            })
            .Build();

        await pipeline.ExecuteAsync(async ct =>
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync(ct);
            if (!pendingMigrations.Any())
            {
                return;
            }

            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync(ct);
            logger.LogInformation("Database migration successful.");
        });
    }
}