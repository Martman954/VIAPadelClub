using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Aggregates.Schedule.ValueObjects;

public class ScheduleTimeInterval
{
    private const int OperatingHourStart = 15;
    private const int OperatingHourEnd = 22;
    private static readonly TimeSpan MinimumDuration = TimeSpan.FromMinutes(60);
    
    public TimeInterval TimeInterval { get; }
    public bool IsVip { get; }

    private ScheduleTimeInterval(TimeInterval timeInterval, bool isVip)
    {
        TimeInterval = timeInterval;
        IsVip = isVip;
    }

    public static Result<ScheduleTimeInterval> Create(TimeInterval timeInterval, bool isVip)
        => Result.Combine(
            ValidateOperatingHours(timeInterval),
            ValidateMinimumDuration(timeInterval)
        ).WithSuccessPayload(new ScheduleTimeInterval(timeInterval, isVip));

    private static Result<None> ValidateOperatingHours(TimeInterval timeInterval)
    {
        var startHour = timeInterval.Start.Hour;
        var endHour = timeInterval.End.Hour;
        var endMinute = timeInterval.End.Minute;

        if (startHour < OperatingHourStart)
            return Result.Failure(
                $"Schedule start time must be at or after {OperatingHourStart}:00.",
                ErrorType.Validation);

        if (endHour > OperatingHourEnd || (endHour == OperatingHourEnd && endMinute > 0))
            return Result.Failure(
                $"Schedule end time must be at or before {OperatingHourEnd}:00.",
                ErrorType.Validation);

        return Result.Success();
    }

    private static Result<None> ValidateMinimumDuration(TimeInterval timeInterval) =>
        timeInterval.Duration < MinimumDuration
            ? Result.Failure(
                $"Schedule time interval must be at least {(int)MinimumDuration.TotalMinutes} minutes.",
                ErrorType.Validation)
            : Result.Success();
}