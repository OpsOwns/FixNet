using System.Text.Json;
using FixNet.Domain.Base;
using FixNet.Infrastructure.EventDispatcher;
using FixNet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FixNet.Infrastructure.OutBox;

public class OutboxProcessor(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxProcessor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();

            var dispatcher = scope.ServiceProvider.GetRequiredService<IDomainEventDispatcher>();
            var dbContext = scope.ServiceProvider.GetRequiredService<FixNetDbContext>();

            var messages = await dbContext.OutboxMessages
                .Where(m => m.ProcessedOnUtc == null)
                .OrderBy(m => m.OccurredOnUtc)
                .Take(20)
                .ToListAsync(stoppingToken);

            if (messages.Count == 0)
            {
                await Task.Delay(5000, stoppingToken);
                continue;
            }

            foreach (var message in messages)
            {
                try
                {
                    var type = Type.GetType(message.Type);
                    if (type == null)
                    {
                        logger.LogError("Could not resolve domain event type: {Type}", message.Type);
                        message.Error = $"Could not resolve type: {message.Type}";
                        continue;
                    }

                    if (JsonSerializer.Deserialize(message.Content, type) is IDomainEvent domainEvent)
                    {
                        await dispatcher.Dispatch(domainEvent, stoppingToken);
                    }

                    message.ProcessedOnUtc = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    message.Error = ex.Message;
                }
            }

            await dbContext.SaveChangesAsync(stoppingToken);
            await Task.Delay(5000, stoppingToken);
        }
    }
}