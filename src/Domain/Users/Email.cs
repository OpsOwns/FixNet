using System.ComponentModel.DataAnnotations;
using FixNet.Domain.Base;

namespace FixNet.Domain.Users;

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