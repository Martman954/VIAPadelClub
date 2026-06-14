namespace VIAPadelClub.Core.Domain.Common;

public abstract class Id<T> where T : Id<T>
{
    public abstract object Value { get; }
    
    public override bool Equals(object? obj)
    {
        return obj is T other && Value.Equals(other.Value);
    }
    
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

