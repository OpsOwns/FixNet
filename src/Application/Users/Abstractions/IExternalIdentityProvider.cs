using FixNet.Application.Abstractions;

namespace FixNet.Application.Users.Abstractions;

public interface IExternalIdentityProvider
{
    Task<string> CreateUserAsync(CreateIdentityRequest request, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(string externalUserId, CancellationToken cancellationToken = default);
}