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
    public ScheduleTimeInterval Times { get; private set; }
    public Status Status { get; private set; }
    
    private List<ScheduleTimeInterval> _activeTimeSlots;
    public IReadOnlyList<ScheduleTimeInterval> ActiveTime => _activeTimeSlots.AsReadOnly();

    private List<CourtId> _courts;
    public IReadOnlyList<CourtId> Courts => _courts.AsReadOnly();

    private Schedule(Guid id, ScheduleTimeInterval times, List<ScheduleTimeInterval> activeTimeSlots)
    {
        Id = id;
        Status = Status.Draft;
        Times = times;
        _courts = [];
        _activeTimeSlots = new List<ScheduleTimeInterval>(activeTimeSlots);
    }
    
    public static Result<Schedule> Create(ScheduleTimeInterval times, List<ScheduleTimeInterval> activeSlots)
    {
        if (activeSlots == null || activeSlots.Count == 0)
        {
            return Result.Failure<Schedule>(new ResultError(
                "Schedule must contain at least one time interval", 
                ErrorType.Validation));
        }

        return new Schedule(Guid.NewGuid(), times, activeSlots);
    }
    
    public Result<None> UpdateDate(DateTime newDate)
    {
        if (!Status.Equals(Status.Draft))
            return new ResultError("Schedule date can only be updated while in Draft status.", ErrorType.Validation);

        if (newDate.Date < DateTime.Today)
            return new ResultError("Schedule date cannot be set to a date in the past.", ErrorType.Validation);
        
        return None.Value;
    }

    public Result<None> UpdateActiveTime(List<ScheduleTimeInterval> timeIntervals)
    {
        if (!Status.Equals(Status.Draft))
            return new ResultError("Schedule time intervals can only be updated while in Draft status.", ErrorType.Validation);

        if (timeIntervals == null || timeIntervals.Count == 0)
            return new ResultError("Schedule must contain at least one time interval.", ErrorType.Validation);

        _activeTimeSlots = new List<ScheduleTimeInterval>(timeIntervals);
        return None.Value;
    }

    public Result<None> AddCourt(CourtId courtId)
    {
        if (Status is not (Status.Draft or Status.Active))
            return new ResultError("Courts can only be added to draft or active schedules.", ErrorType.Validation);
        
        if (Times.TimeInterval.Start.Date <= DateTime.Today)
            return new ResultError("Courts can only be added to future schedules.", ErrorType.Validation);

        if (_courts.Contains(courtId))
            return new ResultError("This court is already added to the schedule.", ErrorType.Validation);
        
        _courts.Add(courtId);
        return None.Value;
    }

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