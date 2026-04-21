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

    private Schedule(Guid id, ScheduleTimeInterval times)
    {
        Id = id;
        Status = Status.Draft;
        _courts = [];
        _times = [times];
    }
    
    public static Result<Schedule> Create(ScheduleTimeInterval times, List<ScheduleTimeInterval> intervals)
    {
        return new Schedule(Guid.NewGuid(), times);
    }
    
    public Result<None> UpdateDate(DateTime newDate)
    {
        if (!Status.Equals(Status.Draft))
            return new ResultError("Schedule date can only be updated while in Draft status.", ErrorType.Validation);

        if (newDate.Date < DateTime.Today)
            return new ResultError("Schedule date cannot be set to a date in the past.", ErrorType.Validation);
        
        return Result.Success();
    }

    public Result<None> UpdateActiveTime(List<ScheduleTimeInterval> timeIntervals)
    {
        if (!Status.Equals(Status.Draft))
            return new ResultError("Schedule time intervals can only be updated while in Draft status.", ErrorType.Validation);

        if (timeIntervals == null || timeIntervals.Count == 0)
            return new ResultError("Schedule must contain at least one time interval.", ErrorType.Validation);

        _times = new List<ScheduleTimeInterval>(timeIntervals);
        return Result.Success();
    }

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