using FixNet.Domain.Abstractions;
using FixNet.Domain.Primitives;

namespace FixNet.Application.Common.Abstractions;

public interface IAggregateStore
{
    Task<Result<TAggregate>> LoadAsync<TAggregate, TId>(TId id, CancellationToken cancellationToken = default) where TAggregate : class, IAggregateRoot where TId : notnull;
    void Add<TAggregate>(TAggregate aggregate) where TAggregate : class, IAggregateRoot;
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}