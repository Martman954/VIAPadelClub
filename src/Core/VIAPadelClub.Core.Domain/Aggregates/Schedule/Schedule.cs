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
    
    public static Result<Schedule> Create(ScheduleTimeInterval times)
    {
        return new Schedule(Guid.NewGuid(), times);
    }

    public Result<None> UpdateTimes(ScheduleTimeInterval scheduleTimeInterval)
    {
        if (!Status.Equals(Status.Draft))
            return new ResultError("Schedule time intervals can only be updated while in Draft status.", ErrorType.Validation);

        _times = [scheduleTimeInterval];
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