using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Common.Values;

public class TimeInterval
{
    public DateTime Start { get; }
    public DateTime End { get; }
    public TimeSpan Duration => End.Subtract(Start);

    private TimeInterval(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    public static Result<TimeInterval> Create(DateTime start, DateTime end)
        =>
            Result.Combine(
                TimeIntervalMustBeInCorrectFormat(start, end),
                TimeIntervalMustBeWithinSameDay(start, end)
            ).WithSuccessPayload(new TimeInterval(start, end));


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