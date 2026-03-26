namespace FixNet.Domain.Users;

internal static class UserFactory
{
    public static User Load(UserId id, ExternalId externalId, UserType type, FirstName firstName,
        LastName lastName, Email email, Phone phoneNumber, bool isAvailable)
        => new(id, externalId, type, firstName, lastName, email, phoneNumber, isAvailable);
}