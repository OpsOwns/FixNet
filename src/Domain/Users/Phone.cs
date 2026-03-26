using System.Text.RegularExpressions;
using FixNet.Domain.Base;

namespace FixNet.Domain.Users;

public sealed partial class Phone : ValueObject
{
    private const string Pattern = @"^\d{9}$";
    public string Value { get; }

    private Phone(string value)
    {
        Value = value;
    }

    public static Result<Phone> Create(string number)
    {
        if (string.IsNullOrEmpty(number))
            return Error.New("User.PhoneNumber", "Phone number is required");

        if (!PhoneRegex().IsMatch(number))
            return Error.New("User.PhoneNumber", "Invalid phone number");

        return new Phone(number);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex(Pattern)]
    private static partial Regex PhoneRegex();
}