using VIAPadelClub.Core.Domain.Common.Bases;

namespace VIAPadelClub.Core.Domain.Common.Values.Schedule;

public class ScheduleTimeInterval
{
    public TimeInterval TimeInterval { get; }
    public bool IsVip { get; }

    public ScheduleTimeInterval(TimeInterval timeInterval, bool isVip)
    {
        TimeInterval = timeInterval;
        IsVip = isVip;
    }
}