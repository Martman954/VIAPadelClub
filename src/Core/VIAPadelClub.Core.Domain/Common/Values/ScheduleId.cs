namespace VIAPadelClub.Core.Domain.Common.Values;

/// <summary>
/// Strongly-typed identifier for Schedule aggregates
/// </summary>
public sealed class ScheduleId : Id<ScheduleId>
{
    public Guid GuidValue { get; }

    private ScheduleId(Guid value)
    {
        GuidValue = value;
    }

    public override object Value => GuidValue;

    /// <summary>
    /// Creates a new unique ScheduleId
    /// </summary>
    public static ScheduleId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a ScheduleId from an existing Guid
    /// </summary>
    public static ScheduleId From(Guid value) => new(value);

    public override string ToString() => GuidValue.ToString();
}


