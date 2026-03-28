using FixNet.Domain.Base;

namespace FixNet.Domain.Users.Abstractions;

public interface IUserRepository : IRepository
{
    Task CreateUserAsync(User user, CancellationToken cancellationToken);
}