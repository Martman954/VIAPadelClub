namespace VIAPadelClub.Core.Domain.Common.Values.Schedule;

public class TimeInterval
{
    public DateTime Start { get; }
    public DateTime End { get; }

    public TimeInterval(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new ArgumentException("End must be after start");

        Start = start;
        End = end;
    }
}