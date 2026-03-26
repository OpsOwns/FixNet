namespace FixNet.Domain.Users;

public readonly record struct UserId(Guid Value)
{
    public static UserId Empty => new(Guid.Empty);
    public static UserId Create() => new(Guid.CreateVersion7());

    public static UserId Assignee(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}