namespace FixNet.Application.Common.Abstractions;

public interface IKeycloakUserService
{
    Task<string> CreateUserAsync(ExternalIdentity request, CancellationToken cancellationToken = default);
    Task AssignRoleAsync(string userId, Role roleName, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(string externalUserId, CancellationToken cancellationToken = default);
    Task SendActivationEmailAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
}