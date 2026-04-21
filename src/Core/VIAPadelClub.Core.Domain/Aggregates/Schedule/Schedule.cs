using VIAPadelClub.Core.Domain.Aggregates.Schedule.Enums;
using VIAPadelClub.Core.Domain.Aggregates.Schedule.ValueObjects;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Aggregates.Schedule;

/// <summary>
/// Aggregate root representing the availability schedule for padel courts
/// </summary>
public sealed class Schedule
{
    public Guid Id { get; }
    public Status Status { get; private set; }

    private List<ScheduleTimeInterval> _times;
    public IReadOnlyList<ScheduleTimeInterval> Times => _times.AsReadOnly();

    public IReadOnlyList<ScheduleTimeInterval> VipTimes
        => _times.Where(s => s.IsVip).ToList().AsReadOnly();

    public IReadOnlyList<ScheduleTimeInterval> RegularTimes
        => _times.Where(s => !s.IsVip).ToList().AsReadOnly();

    private List<CourtId> _courts;
    public IReadOnlyList<CourtId> Courts => _courts.AsReadOnly();

    private static readonly TimeOnly DefaultStart = new(15, 0);
    private static readonly TimeOnly DefaultEnd = new(22, 0);

    private Schedule(Guid id, ScheduleTimeInterval defaultTime)
    {
        Id = id;
        Status = Status.Draft;
        _courts = [];
        _times = [defaultTime];
    }


    /// <summary>
    /// ID: 1
    /// I want to create a new daily schedule
    /// When manager selects to create a daily schedule
    ///     Then a daily schedule is created with an ID
    ///     And the status is set to “draft”
    ///     And the list of available courts is empty
    ///     And the times are set to 15:00 and 22:00
    ///     And date is set to today
    /// </summary>
    public static Result<Schedule> Create()
    {
        var today = DateTime.Today;
        var start = today.Add(DefaultStart.ToTimeSpan());
        var end = today.Add(DefaultEnd.ToTimeSpan());

        var timeIntervalResult = TimeInterval.Create(start, end);
        if (timeIntervalResult is Result<TimeInterval>.Failure f1)
            return Result.Failure<Schedule>(f1.Errors);

        var timeInterval = ((Result<TimeInterval>.Success)timeIntervalResult).Value;

        var scheduleTimeIntervalResult = ScheduleTimeInterval.Create(timeInterval, false);
        if (scheduleTimeIntervalResult is Result<ScheduleTimeInterval>.Failure f2)
            return Result.Failure<Schedule>(f2.Errors);

        var scheduleTimeInterval = ((Result<ScheduleTimeInterval>.Success)scheduleTimeIntervalResult).Value;

        return new Schedule(Guid.NewGuid(), scheduleTimeInterval);
    }

    /// <summary>
    /// ID: 2 S:1
    /// I want to create a new daily schedule, date
    /// </summary>
    public Result<None> UpdateDate(DateTime newDate)
    {
        if (!Status.Equals(Status.Draft))
            return new ResultError("Schedule date can only be updated while in Draft status.", ErrorType.Validation);

        if (newDate.Date < DateTime.Today)
            return new ResultError("Schedule date cannot be set to a date in the past.", ErrorType.Validation);

        return Result.Success();
    }

    /// <summary>
    /// ID: 2 S:2
    /// I want to update a new daily schedule, times
    ///
    /// If part of a schedule is VIP only, _times will have more than one element,
    /// so we need to update the first and last element of the list, keeping the VIP part in between unchanged.
    /// </summary>
    public Result<None> UpdateTimes(TimeInterval timeInterval)
    {
        if (!Status.Equals(Status.Draft))
            return new ResultError("Schedule time intervals can only be updated while in Draft status.",
                ErrorType.Validation);

        // Rebuild first element with new start, keeping its original end
        var firstOriginal = _times[0].TimeInterval;
        var firstIntervalResult = TimeInterval.Create(timeInterval.Start, firstOriginal.End);
        if (firstIntervalResult is Result<TimeInterval>.Failure f1)
            return Result.Failure<None>(f1.Errors);
        var firstInterval = ((Result<TimeInterval>.Success)firstIntervalResult).Value;

        var firstResult = ScheduleTimeInterval.Create(firstInterval, _times[0].IsVip);
        if (firstResult is Result<ScheduleTimeInterval>.Failure f2)
            return Result.Failure<None>(f2.Errors);

        // Rebuild last element with new end, keeping its original start
        var lastOriginal = _times[^1].TimeInterval;
        var lastIntervalResult = TimeInterval.Create(lastOriginal.Start, timeInterval.End);
        if (lastIntervalResult is Result<TimeInterval>.Failure f3)
            return Result.Failure<None>(f3.Errors);
        var lastInterval = ((Result<TimeInterval>.Success)lastIntervalResult).Value;

        var lastResult = ScheduleTimeInterval.Create(lastInterval, _times[^1].IsVip);
        if (lastResult is Result<ScheduleTimeInterval>.Failure f4)
            return Result.Failure<None>(f4.Errors);

        _times[0] = ((Result<ScheduleTimeInterval>.Success)firstResult).Value;
        _times[^1] = ((Result<ScheduleTimeInterval>.Success)lastResult).Value;

        return Result.Success();
    }

    /// <summary>
    /// ID: 3
    /// The manager adds available courts to a daily schedule
    /// </summary>
    public Result<None> AddCourt(CourtId courtId)
    {
        if (Status is not (Status.Draft or Status.Active))
            return new ResultError("Courts can only be added to draft or active schedules.", ErrorType.Validation);

        if (_times.Min(st => st.TimeInterval.Start).Date <= DateTime.Today)
            return new ResultError("Courts can only be added to future schedules.", ErrorType.Validation);

        if (_courts.Contains(courtId))
            return new ResultError("This court is already added to the schedule.", ErrorType.Validation);

        _courts.Add(courtId);
        return Result.Success();
    }

    public Result<None> RemoveCourt(CourtId courtId, Court.Court court, DateTime currentTime)
    {
        var validation = Result.Combine(
            ValidateNotInPast(currentTime),
            ValidateCourtExists(courtId),
            ValidateNoFutureBookings(court, currentTime)
        );

        if (validation is Result<None>.Failure f)
            return f;

        _courts.Remove(courtId);
        return Result.Success();
    }

    private Result<None> ValidateNotInPast(DateTime currentTime) =>
        _times.Min(st => st.TimeInterval.Start).Date < currentTime.Date
            ? Result.Failure("Past daily schedules cannot be modified.", ErrorType.Validation)
            : Result.Success();

    private Result<None> ValidateCourtExists(CourtId courtId) =>
        _courts.Contains(courtId)
            ? Result.Success()
            : Result.Failure("The court was not found in the daily schedule.", ErrorType.NotFound);

    private Result<None> ValidateNoFutureBookings(Court.Court court, DateTime currentTime) =>
        court.Bookings.Any(b => !b.IsCancelled && b.TimeInterval.Start >= currentTime)
            ? Result.Failure("Courts with bookings later on the same day cannot be removed.", ErrorType.Validation)
            : Result.Success();

    public Result<None> Activate()
    {
        Status = Status.Active;
        return None.Value;
    }

    public Result<None> Delete()
    {
        Status = Status.Deleted;
        return None.Value;
    }
}