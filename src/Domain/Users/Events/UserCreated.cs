using FixNet.Domain.Base;

namespace FixNet.Domain.Users.Events;

public record UserCreatedDomainEvent(string KeycloakUserId) : IDomainEvent;