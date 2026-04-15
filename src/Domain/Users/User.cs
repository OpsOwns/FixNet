using FixNet.Domain.Base;
using FixNet.Domain.Users.Events;

namespace FixNet.Domain.Users;

public class User : AggregateRoot
{
    public UserId Id { get; private set; }
    public ExternalId ExternalId { get; private set; }
    public UserType Type { get; private set; }
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Email Email { get; private set; }
    public Phone PhoneNumber { get; private set; }
    public bool IsAvailable { get; private set; }

    internal User(UserId userId, ExternalId externalId, UserType userType,
        FirstName firstName,
        LastName lastName,
        Email email,
        Phone phoneNumber,
        bool isAvailable)
    {
        Id = userId;
        ExternalId = externalId;
        Type = userType;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        IsAvailable = isAvailable;

        RaiseDomainEvent(new UserCreatedDomainEvent(externalId.Value));
    }

    public static User Create(ExternalId externalId, UserType type, FirstName firstName, LastName lastName, Email email, Phone phone) =>
        new(UserId.Create(), externalId, type, firstName, lastName, email, phone, true);

    public void Occupy() => IsAvailable = false;
    public void Release() => IsAvailable = true;
    public bool CanBeAssignedToJob() => Type == UserType.Technic && IsAvailable;
}