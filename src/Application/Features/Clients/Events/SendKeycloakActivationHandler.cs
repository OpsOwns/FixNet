using FixNet.Application.Base.Abstractions;
using FixNet.Domain.Users.Events;

namespace FixNet.Application.Features.Clients.Events;

public class SendKeycloakActivationHandler(IExternalIdentityProvider identityProvider)
    : IDomainEventHandler<UserCreatedDomainEvent>
{
    public async Task Handle(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await identityProvider.SendActivationEmailAsync(domainEvent.KeycloakUserId, cancellationToken);
    }
}