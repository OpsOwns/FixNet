using System.Text.Json;
using FixNet.Application.Common.Abstractions;
using FixNet.Domain.Primitives;
using FixNet.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace FixNet.Infrastructure.Persistence;

public class FixNetDbContext(DbContextOptions<FixNetDbContext> options, TimeProvider timeProvider) : DbContext(options), IAppDbContext
{
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FixNetDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var now = timeProvider.GetUtcNow();

        var domainEvents = ChangeTracker
            .Entries<AggregateRoot>()
            .SelectMany(x => x.Entity.Events)
            .Select(domainEvent => new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = now,
                Type = domainEvent.GetType().FullName ?? throw new InvalidOperationException(
                    "Domain event type name is missing."),
                Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
            })
            .ToList();

        if (domainEvents.Count is 0)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        AddRange(domainEvents);

        foreach (var entry in ChangeTracker.Entries<AggregateRoot>())
        {
            entry.Entity.MarkChangesAsCommitted();
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}