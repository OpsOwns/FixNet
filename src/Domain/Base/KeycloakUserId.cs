namespace FixNet.Domain.Base;

public readonly record struct KeycloakUserId(string Value)
{
    public bool IsEmpty => string.IsNullOrEmpty(Value);
    public static KeycloakUserId Empty => new(string.Empty);
    public static KeycloakUserId Create(string value) => new(value);

    public override string ToString() => Value;

    public static implicit operator KeycloakUserId(string value) => Create(value);
}