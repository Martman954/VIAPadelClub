namespace VIAPadelClub.Core.Domain.Common.Values;

public sealed record CourtId
{
    private string Value { get; set; }
    
    private CourtId(string value)
    {
        Value = value;
    }
    public static CourtId Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("CourtId value cannot be null, empty, or whitespace.", nameof(value));
        }
        return new CourtId(value);
    }
    public override string ToString() => Value;
}