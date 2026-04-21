using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Aggregates.Schedule.ValueObjects;

public class ScheduleTimeInterval
{
    private static readonly TimeOnly OperatingHourStart = new TimeOnly(8, 0);
    private static readonly TimeOnly OperatingHourEnd = new TimeOnly(22, 0);
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
            ValidateDateIsNotInPast(timeInterval),
            ValidateOperatingHours(timeInterval),
            ValidateMinimumDuration(timeInterval)
        ).WithSuccessPayload(new ScheduleTimeInterval(timeInterval, isVip));

    private static Result<None> ValidateDateIsNotInPast(TimeInterval timeInterval) =>
        timeInterval.Start.Date < DateTime.Today
            ? Result.Failure(
                "Schedule date cannot be in the past.",
                ErrorType.Validation)
            : Result.Success();

    private static Result<None> ValidateOperatingHours(TimeInterval timeInterval)
    {
        var start = TimeOnly.FromDateTime(timeInterval.Start);
        var end = TimeOnly.FromDateTime(timeInterval.End);

        if (start < OperatingHourStart)
            return Result.Failure(
                $"Schedule start time must be at or after {OperatingHourStart}.",
                ErrorType.Validation);

        if (end > OperatingHourEnd)
            return Result.Failure(
                $"Schedule end time must be at or before {OperatingHourEnd}.",
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