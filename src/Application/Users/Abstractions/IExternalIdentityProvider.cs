namespace FixNet.Application.Users.Abstractions;

public interface IExternalIdentityProvider
{
    Task<string> CreateUserAsync(CreateIdentityRequest request, CancellationToken cancellationToken = default);
    Task AssignRoleAsync(string userId, string roleName, CancellationToken ct);
    Task DeleteUserAsync(string externalUserId, CancellationToken cancellationToken = default);
}