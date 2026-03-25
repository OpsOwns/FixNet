using System.ComponentModel.DataAnnotations;
using FixNet.Domain.Base;

namespace FixNet.Domain;

public readonly record struct UserId(Guid Value)
{
    public static UserId Empty => new(Guid.Empty);
    public static UserId Create() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}

public enum Role
{
    Client = 1,
    Technic = 2,
    Manager = 3,
    Admin = 4
}

public sealed class Email : ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Error.New(
                "User.EmailRequired", "Email address is required.");

        if (!new EmailAddressAttribute().IsValid(email))
            return Error.New("User.InvalidEmail", "The provided email address is not in a valid format.");

        return new Email(email);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

public class User
{
    public UserId UserId { get; private set; }
    public Role Role { get; private set; }

    public void Hydrate(UserId userId)
    {
        UserId = userId;
    }
}