namespace FixNet.Domain.Base;

public class Error : ValueObject
{
    public string Code { get; }
    public string Message { get; }

    public static readonly Error None = new(string.Empty, string.Empty);

    private Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public static Error New(string code, string message)
        => new(code, message);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
        yield return Message;
    }

    public override string ToString() => $"{Code}: {Message}";
}