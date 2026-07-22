using FixNet.Domain.Primitives;

namespace FixNet.Application.Common.Abstractions;

public interface ICommandHandler<in T> where T : class, ICommand
{
    Task<Result> HandleAsync(T command, CancellationToken cancellationToken = default);
}