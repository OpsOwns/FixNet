using FixNet.Domain.Base;

namespace FixNet.Infrastructure.EventDispatcher;

public interface IDomainEventDispatcher
{
    Task Dispatch(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}