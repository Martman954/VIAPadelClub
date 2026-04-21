using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Aggregates.Schedule.ValueObjects;

public class ScheduleTimeInterval
{
    private static readonly TimeSpan MinimumDuration = TimeSpan.FromMinutes(60);
    
    public TimeInterval TimeInterval { get; }
    public bool IsVip { get; }

    private ScheduleTimeInterval(TimeInterval timeInterval, bool isVip)
    {
        TimeInterval = timeInterval;
        IsVip = isVip;
    }

    public static Result<ScheduleTimeInterval> Create(TimeInterval timeInterval, bool isVip)
        => Result.Combine(ValidateDateIsNotInPast(timeInterval), 
            ValidateMinimumDuration(timeInterval)
        ).WithSuccessPayload(new ScheduleTimeInterval(timeInterval, isVip));

    private static Result<None> ValidateDateIsNotInPast(TimeInterval timeInterval) =>
        timeInterval.Start.Date < DateTime.Today
            ? Result.Failure(
                "Schedule date cannot be in the past.",
                ErrorType.Validation)
            : Result.Success();

    private static Result<None> ValidateMinimumDuration(TimeInterval timeInterval) =>
        timeInterval.Duration < MinimumDuration
            ? Result.Failure(
                $"Schedule time interval must be at least {(int)MinimumDuration.TotalMinutes} minutes.",
                ErrorType.Validation)
            : Result.Success();
}