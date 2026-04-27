using FixNet.Application.Common.Abstractions;
using FixNet.Domain.Base;
using Microsoft.Extensions.Logging;

namespace FixNet.Application.Features.Users.CreateUser;

internal sealed class CreateUserHandler(IKeycloakUserService keycloakUserService, ILogger<CreateUserHandler> logger) : ICommandHandler<CreateUserCommand>
{
    public async Task<Result> HandleAsync(CreateUserCommand createUserCommand, CancellationToken cancellationToken = default)
    {
        var keycloakUserId = KeycloakUserId.Empty;
        try
        {
            if (await keycloakUserService.EmailExistsAsync(createUserCommand.Email, cancellationToken))
            {
                return Result.Failure(Error.New("User.EmailAlreadyExists", $"Email '{createUserCommand.Email}' already in use"));
            }

            var identity = new ExternalIdentity(createUserCommand.Email, createUserCommand.Password,
                createUserCommand.FirstName, createUserCommand.LastName, createUserCommand.Phone);

            keycloakUserId = await keycloakUserService.CreateUserAsync(identity, cancellationToken);

            await keycloakUserService.AssignRoleAsync(keycloakUserId.Value, createUserCommand.Role, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to create user {Email}.", createUserCommand.Email);

            if (keycloakUserId.IsEmpty)
            {
                await keycloakUserService.DeleteUserAsync(keycloakUserId.Value, cancellationToken);
            }

            return Result.Failure(Error.New(
                "User.CreationFailed",
                "Failed to create user account"));
        }
    }
}