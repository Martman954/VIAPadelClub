namespace VIAPadelClub.Core.Domain.Common;

/// <summary>
/// Base class for all strongly-typed identifiers in the domain model.
/// This ensures type safety for IDs and prevents mixing up different ID types.
/// </summary>
/// <typeparam name="T">The concrete type implementing this base class (self-referential generic)</typeparam>
public abstract class Id<T> where T : Id<T>
{
    /// <summary>
    /// The underlying value of the identifier
    /// </summary>
    public abstract object Value { get; }

    /// <summary>
    /// Determines equality based on the underlying value
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not T other)
            return false;

        return Value.Equals(other.Value);
    }

    /// <summary>
    /// Returns hash code based on the underlying value
    /// </summary>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

