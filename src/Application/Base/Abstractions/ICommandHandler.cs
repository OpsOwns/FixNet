using FixNet.Domain.Base;

namespace FixNet.Application.Base.Abstractions;

public interface ICommandHandler<in T> where T : class, ICommand
{
    Task<Result> HandleAsync(T command, CancellationToken cancellationToken = default);
}