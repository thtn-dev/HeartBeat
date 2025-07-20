namespace ZSocialMedia.Shared;

[Serializable]
public abstract class ValueObject : IEquatable<ValueObject>
{
    protected static bool EqualOperator(ValueObject? left, ValueObject? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    protected static bool NotEqualOperator(ValueObject? left, ValueObject? right)
    {
        return !EqualOperator(left, right);
    }

    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj == null || obj.GetType() != GetType()) return false;

        return Equals((ValueObject)obj);
    }

    public bool Equals(ValueObject? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        var components = GetEqualityComponents();

        return components
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate(17, (hash, next) => hash * 31 + next);
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        return EqualOperator(left, right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return NotEqualOperator(left, right);
    }

    public virtual ValueObject GetCopy()
    {
        return (ValueObject)MemberwiseClone();
    }
}