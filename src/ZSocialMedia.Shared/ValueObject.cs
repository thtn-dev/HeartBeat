﻿namespace ZSocialMedia.Shared;

[Serializable]
public abstract class ValueObject
{
    protected static bool EqualOperator(ValueObject left, ValueObject right)
    {
        if (left is null ^ right is null)
        {
            return false;
        }
        return right != null && (left is null || left.Equals(right));
    }

    protected static bool NotEqualOperator(ValueObject left, ValueObject right)
    {
        return !(EqualOperator(left, right));
    }

    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
             .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate(0, (x, y) => x ^ y);
    }

    public ValueObject? GetCopy()
    {
        return MemberwiseClone() as ValueObject;
    }
    
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        return left != null && right != null && EqualOperator(left, right);
    }
    
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null) return false;
        return NotEqualOperator(left!, right!);
    }
}