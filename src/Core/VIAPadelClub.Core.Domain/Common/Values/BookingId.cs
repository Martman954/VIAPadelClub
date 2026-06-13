namespace VIAPadelClub.Core.Domain.Common.Values;

public sealed record BookingId
{
    public Guid Value { get; }

    private BookingId(Guid value)
    {
        Value = value;
    }

    public static BookingId New() => new(Guid.NewGuid());
    
    public static BookingId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}