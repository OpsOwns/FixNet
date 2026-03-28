namespace FixNet.Domain.Users;

public readonly record struct ExternalId(string Value)
{
    public static ExternalId Empty => new(string.Empty);
    public static ExternalId Create(string value) => new(value);

    public override string ToString() => Value;

    public static implicit operator ExternalId(string value) => Create(value);
}