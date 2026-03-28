using FixNet.Application.Base;
using FixNet.Application.Base.Abstractions;
using FixNet.Domain.Base;
using FixNet.Domain.Users;
using FixNet.Domain.Users.Abstractions;
using Microsoft.Extensions.Logging;

namespace FixNet.Application.Features.Clients.CreateClient;

internal sealed class CreateClientHandler(IExternalIdentityProvider externalIdentityProvider, ILogger<CreateClientHandler> logger, IUserRepository userRepository) : ICommandHandler<CreateClientCommand>
{
    public async Task<Result> HandleAsync(CreateClientCommand createClientCommand, CancellationToken cancellationToken = default)
    {
        var firstName = FirstName.Create(createClientCommand.FirstName);
        var lastName = LastName.Create(createClientCommand.LastName);
        var phone = Phone.Create(createClientCommand.PhoneNumber);
        var email = Email.Create(createClientCommand.Email);
        var password = Password.Create(createClientCommand.Password);

        var combinedResult = Result.Combine(firstName, lastName, phone, email, password);

        if (combinedResult.IsFailure)
        {
            return combinedResult;
        }

        ExternalId? externalId = null;

        try
        {
            var identity = new ExternalIdentity(email.Value, password.Value, firstName.Value, lastName.Value);

            externalId = await externalIdentityProvider.CreateUserAsync(identity, cancellationToken);

            await externalIdentityProvider.AssignRoleAsync(externalId.Value.Value, nameof(UserType.Client), cancellationToken);

            var user = User.Create(externalId.Value, UserType.Client, firstName.Value, lastName.Value, email.Value, phone.Value);

            await userRepository.CreateUserAsync(user, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to create client {Email}.", createClientCommand.Email);

            if (externalId.HasValue)
            {
                await externalIdentityProvider.DeleteUserAsync(externalId.Value.Value, cancellationToken);
            }

            return Result.Failure(Error.New(
                "Client.CreationFailed",
                "Failed to create client account"));
        }
    }
}