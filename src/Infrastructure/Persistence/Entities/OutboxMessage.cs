using System.Text.Json;
using FixNet.Domain.Abstractions;

namespace FixNet.Infrastructure.Persistence.Entities;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset OccurredOnUtc { get; set; }
    public string? Error { get; set; }
    public DateTime? ProcessedOnUtc { get; set; }

    public static OutboxMessage ToOutboxMessage(IDomainEvent domainEvent) =>
        new()
        {
            Id = Guid.NewGuid(),
            OccurredOnUtc = DateTime.UtcNow,
            Type = domainEvent.GetType().FullName ?? throw new InvalidOperationException("Domain event has no type"),
            Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
        };
}