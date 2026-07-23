using FixNet.Application.Common.Abstractions;
using FixNet.Domain.Abstractions;
using FixNet.Domain.Primitives;

namespace FixNet.Infrastructure.Persistence;

internal sealed class AggregateStore(FixNetDbContext dbContext) : IAggregateStore
{
    public async Task<Result<TAggregate>> LoadAsync<TAggregate, TId>(
        TId id,
        CancellationToken cancellationToken = default)
        where TAggregate : class, IAggregateRoot
        where TId : notnull
    {
        var aggregate = await dbContext.Set<TAggregate>()
            .FindAsync([id], cancellationToken);

        if (aggregate is null)
        {
            return Result<TAggregate>.Failure(
                Error.New(
                    $"{typeof(TAggregate).Name}.NotFound",
                    $"{typeof(TAggregate).Name} with id '{id}' was not found."));
        }

        return aggregate;
    }

    public void Add<TAggregate>(TAggregate aggregate) where TAggregate : class, IAggregateRoot
    {
        dbContext.Set<TAggregate>().Add(aggregate);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}