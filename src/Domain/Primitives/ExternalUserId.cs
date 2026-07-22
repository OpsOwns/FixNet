namespace FixNet.Domain.Primitives;

public readonly record struct ExternalUserId(string Value)
{
    public bool IsEmpty => string.IsNullOrEmpty(Value);
    public static ExternalUserId Empty => new(string.Empty);
    public static ExternalUserId Create(string value) => new(value);
    public override string ToString() => Value;
}