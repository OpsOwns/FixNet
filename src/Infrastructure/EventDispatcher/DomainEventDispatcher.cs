using FixNet.Application.Base.Abstractions;
using FixNet.Domain.Base;
using Microsoft.Extensions.DependencyInjection;

namespace FixNet.Infrastructure.EventDispatcher;

internal sealed class DomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    public async Task Dispatch(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handlers = serviceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            var task = (Task)handlerType
                .GetMethod(nameof(IDomainEventHandler<>.Handle))!
                .Invoke(handler, [domainEvent, cancellationToken])!;

            await task;
        }
    }
}