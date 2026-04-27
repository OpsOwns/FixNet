using System.Text.Json;
using FixNet.Domain.Base;

namespace FixNet.Infrastructure.Persistence.Entities;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime OccurredOnUtc { get; set; }
    public string? Error { get; set; }
    public DateTime? ProcessedOnUtc { get; set; }

    public static OutboxMessage ToOutboxMessage(IDomainEvent domainEvent) =>
        new()
        {
            Id = Guid.NewGuid(),
            OccurredOnUtc = DateTime.UtcNow,
            Type = domainEvent.GetType().AssemblyQualifiedName!,
            Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
        };
}