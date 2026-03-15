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

    private List<CourtId> courts = new();
    public IReadOnlyList<CourtId> Courts => courts.AsReadOnly();

    private Schedule(Guid id, DateTime date, List<ScheduleTimeInterval> activeTime)
    {
        Id = id;
        Date = date;
        _activeTime = new List<ScheduleTimeInterval>(activeTime);
        Status = Status.Inactive;
    }
    
    public static Result<Schedule> Create(DateTime date, List<ScheduleTimeInterval> intervals)
    {
        if (intervals == null || intervals.Count == 0)
        {
            return Result.Failure<Schedule>(new ResultError(
                "Schedule must contain at least one time interval", 
                ErrorType.Validation));
        }

        return new Schedule(Guid.NewGuid(), date, intervals);
    }
    
    public Result<None> UpdateDate(DateTime newDate)
    {
        Date = newDate;
        return None.Value;
    }

    public Result<None> UpdateActiveTime(List<ScheduleTimeInterval> timeIntervals)
    {
        if (timeIntervals == null || timeIntervals.Count == 0)
        {
            return new ResultError("ActiveTime cannot be empty", ErrorType.Validation);
        }

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