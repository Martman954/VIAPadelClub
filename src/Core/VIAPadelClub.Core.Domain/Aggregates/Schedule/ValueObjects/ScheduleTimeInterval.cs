using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Aggregates.Schedule.ValueObjects;

public class ScheduleTimeInterval(TimeInterval timeInterval, bool isVip)
{
    public TimeInterval TimeInterval { get; } = timeInterval;
    public bool IsVip { get; } = isVip;
}