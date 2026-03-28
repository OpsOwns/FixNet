using FixNet.Domain.Base;

namespace FixNet.Domain.Users;

public sealed class LastName : ValueObject
{
    public string Value { get; }

    private LastName(string value)
    {
        Value = value;
    }

    public static Result<LastName> Create(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            return Error.New(
                "User.LastNameRequired",
                "Last name is required.");

        if (lastName.Length is < 2 or > 50)
            return Error.New(
                "User.InvalidLastNameLength",
                "Last name must be between 2 and 50 characters.");

        if (!lastName.All(char.IsLetter))
            return Error.New(
                "User.InvalidLastNameCharacters",
                "Last name must contain only letters.");

        return new LastName(lastName);
    }

    public static implicit operator string(LastName lastName) => lastName.Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}