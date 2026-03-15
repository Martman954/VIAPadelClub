namespace VIAPadelClub.Core.Domain.Common.Values;

public sealed record BookingId
{
    private Guid Value { get; }

    private BookingId(Guid value)
    {
        Value = value;
    }

    public static BookingId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}