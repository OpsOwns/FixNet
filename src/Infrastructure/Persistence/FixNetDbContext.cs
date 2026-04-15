using System.Text.Json;
using FixNet.Domain.Base;
using FixNet.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace FixNet.Infrastructure.Persistence;

public class FixNetDbContext(DbContextOptions<FixNetDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FixNetDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var domainEvents = ChangeTracker
            .Entries<AggregateRoot>()
            .SelectMany(x => x.Entity.Events)
            .Select(domainEvent => new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().AssemblyQualifiedName!,
                Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
            })
            .ToList();

        AddRange(domainEvents);

        foreach (var entry in ChangeTracker.Entries<AggregateRoot>())
        {
            entry.Entity.MarkChangesAsCommitted();
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}