namespace Shared.Domain.Core;

/// <summary>
/// Base value object - immutable, compared by value
/// </summary>
public abstract class BaseValueObject : IEquatable<BaseValueObject>
{
    public abstract bool Equals(BaseValueObject? other);

    public override bool Equals(object? obj)
    {
        return obj is BaseValueObject valueObject && Equals(valueObject);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(BaseValueObject? left, BaseValueObject? right)
    {
        return left?.Equals(right) ?? false;
    }

    public static bool operator !=(BaseValueObject? left, BaseValueObject? right)
    {
        return !(left == right);
    }
}