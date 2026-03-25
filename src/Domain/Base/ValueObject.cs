namespace FixNet.Domain.Base;

public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public bool Equals(ValueObject? other)
    {
        if (other is null || GetType() != other.GetType())
            return false;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return GetType() == obj.GetType() && Equals(obj as ValueObject);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var component in GetEqualityComponents())
        {
            hash.Add(component);
        }

        return hash.ToHashCode();
    }

    private static bool EqualOperator(ValueObject? left, ValueObject? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;

        return left.Equals(right);
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
        => EqualOperator(left, right);

    public static bool operator !=(ValueObject? left, ValueObject? right)
        => !(left == right);

    public override string ToString()
        => string.Join(" | ", GetEqualityComponents());
}