using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Common.Values;

public class TimeInterval
{
    public DateTime Start { get; private set; }
    public DateTime End   { get; private set; }
    public TimeSpan Duration => End.Subtract(Start);

    private TimeInterval(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    /// <summary>For EF Core use only.</summary>
    private TimeInterval() { }

    public static Result<TimeInterval> Create(DateTime start, DateTime end)
        =>
            Result.Combine(
                TimeIntervalMustBeInCorrectFormat(start, end),
                TimeIntervalMustBeWithinSameDay(start, end)
            ).WithSuccessPayload(new TimeInterval(start, end));

    public static Result<TimeInterval> CreateFromDuration(DateTime start, TimeSpan duration)
        => Create(start, start + duration);


    //  Can add more validation
    private static Result<None> TimeIntervalMustBeInCorrectFormat(DateTime start, DateTime end) =>
        (start > end)
            ? Result.Failure("Time interval not in correct format", ErrorType.Validation)
            : Result.Success();

    private static Result<None> TimeIntervalMustBeWithinSameDay(DateTime start, DateTime end) =>
        (start.Date != end.Date)
            ? Result.Failure("Time interval must be within the same day.", ErrorType.Validation)
            : Result.Success();
}