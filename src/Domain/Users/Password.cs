using System.Text.RegularExpressions;
using FixNet.Domain.Base;

namespace FixNet.Domain.Users;

public sealed partial class Password : ValueObject
{
    public string Value { get; }

    private const string PasswordComplexityExpression = @"^(?=.*[a-zA-Z])(?=.*\d.*\d)(?=.*[!@#$%^&*~/""()_=+\[\]\\|,.?-]).*$";
    private const int MinLength = 8;
    private const int MaxLength = 50;

    private Password(string value)
    {
        Value = value;
    }

    public static Result<Password> Create(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Error.New("Users.InvalidPassword", "Password is required.");
        }

        if (value.Length is < MinLength or > MaxLength)
        {
            return Error.New("User.InvalidPasswordLength", $"Password length must be between {MinLength} and {MaxLength} characters.");
        }

        if (!PasswordRegex().IsMatch(value))
        {
            return Error.New("User.InvalidPasswordComplexity", "Password must contain at least one letter, two digits, and one special character.");
        }

        return new Password(value);
    }

    public static implicit operator string(Password password) => password.Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex(PasswordComplexityExpression)]
    private static partial Regex PasswordRegex();
}