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
    public DateTime Date { get; private set; }
    public Status Status { get; private set; }
    
    private List<ScheduleTimeInterval> _activeTime;
    public IReadOnlyList<ScheduleTimeInterval> ActiveTime => _activeTime.AsReadOnly();

    public IReadOnlyList<CourtId> Courts;

    private Schedule(Guid id,  List<ScheduleTimeInterval> activeTime)
    {
        Id = id;
        Date = DateTime.Now;
        _activeTime = new List<ScheduleTimeInterval>(activeTime);
        Status = Status.Draft;
        Courts = [];
    }
    
    public static Result<Schedule> Create(List<ScheduleTimeInterval>? intervals)
    {
        if (intervals == null || intervals.Count == 0)
        {
            return Result.Failure<Schedule>(new ResultError(
                "Schedule must contain at least one time interval", 
                ErrorType.Validation));
        }

        return Result.Success(new Schedule(Guid.NewGuid(), intervals));
    }
    
    public Result<None> UpdateDate(DateTime newDate)
    {
        if (!Status.Equals(Status.Draft))
            return new ResultError("Schedule date can only be updated while in Draft status.", ErrorType.Validation);

        if (newDate.Date < DateTime.Today)
            return new ResultError("Schedule date cannot be set to a date in the past.", ErrorType.Validation);

        Date = newDate;
        return None.Value;
    }

    public Result<None> UpdateActiveTime(List<ScheduleTimeInterval> timeIntervals)
    {
        if (!Status.Equals(Status.Draft))
            return new ResultError("Schedule time intervals can only be updated while in Draft status.", ErrorType.Validation);

        if (timeIntervals == null || timeIntervals.Count == 0)
            return new ResultError("Schedule must contain at least one time interval.", ErrorType.Validation);

        _activeTime = new List<ScheduleTimeInterval>(timeIntervals);
        return None.Value;
    }

    // public Result<None> AddCourt(CourtId courtId)
    // {
    //     courts.Add(courtId);
    //     return None.Value;
    // }

    // public Result<None> RemoveCourt(CourtId courtId)
    // {
    //     courts.Remove(courtId);
    //     return None.Value;
    // }

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