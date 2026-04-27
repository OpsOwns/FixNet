using FixNet.Domain.Base;

namespace FixNet.Application.Common.Abstractions;

public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task Handle(TEvent domainEvent, CancellationToken cancellationToken = default);
}