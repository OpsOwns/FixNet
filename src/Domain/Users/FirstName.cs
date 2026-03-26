using FixNet.Domain.Base;

namespace FixNet.Domain.Users;

public sealed class FirstName : ValueObject
{
    public string Value { get; }

    private FirstName(string value)
    {
        Value = value;
    }

    public static Result<FirstName> Create(string firstName)
    {
        if (string.IsNullOrEmpty(firstName))
            return Error.New(
                "User.FirstNameRequired", "FirstName is required.");

        if (firstName.Length is < 2 or > 50)
            return Error.New("User.InvalidFirstNameLength",
                "First name must be between 2 and 50 characters.");

        if (!firstName.All(char.IsLetter))
            return Error.New(
                "User.InvalidFirstNameCharacters",
                "First name must contain only letters.");

        return new FirstName(firstName);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}