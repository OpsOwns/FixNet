namespace FixNet.Application.Base.Abstractions;

public interface IExternalIdentityProvider
{
    Task<string> CreateUserAsync(ExternalIdentity request, CancellationToken cancellationToken = default);
    Task AssignRoleAsync(string userId, string roleName, CancellationToken ct);
    Task DeleteUserAsync(string externalUserId, CancellationToken cancellationToken = default);
}