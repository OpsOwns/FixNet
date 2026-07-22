using FixNet.Application.Common.Abstractions;
using FixNet.Domain.Base;
using Microsoft.Extensions.Logging;

namespace FixNet.Application.Features.Users.CreateUser;

internal sealed class CreateUserHandler(IKeycloakUserService keycloakUserService, ILogger<CreateUserHandler> logger) : ICommandHandler<CreateUserCommand>
{
    public async Task<Result> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        if (await keycloakUserService.EmailExistsAsync(command.Email, cancellationToken))
        {
            return Result.Failure(Error.New("User.EmailAlreadyExists", $"Email '{command.Email}' already in use"));
        }

        var identity = new ExternalIdentity(command.Email, command.Password,
            command.FirstName, command.LastName, command.Phone);

        KeycloakUserId keycloakUserId = await keycloakUserService.CreateUserAsync(identity, cancellationToken);

        try
        {
            await keycloakUserService.AssignRoleAsync(
                keycloakUserId.Value,
                command.Role,
                cancellationToken);
        }
        catch
        {
            await TryRollbackAsync(keycloakUserId);
            throw;
        }

        return Result.Success();
    }

    private async Task TryRollbackAsync(KeycloakUserId keycloakUserId)
    {
        try
        {
            await keycloakUserService.DeleteUserAsync(
                keycloakUserId.Value,
                CancellationToken.None);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to rollback Keycloak user {UserId}.",
                keycloakUserId.Value);
        }
    }
}