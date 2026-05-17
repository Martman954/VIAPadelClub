using VIAPadelClub.Core.Domain.Aggregates.Schedule.Enums;
using VIAPadelClub.Core.Domain.Aggregates.Schedule.ValueObjects;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Schedule;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Aggregates.Schedule;

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
    
    public Result<None> Activate(IScheduleDateConflictChecker conflictChecker)
    {
        if (Status == Status.Deleted)
            return new ResultError("A deleted schedule cannot be activated.", ErrorType.Validation);
        
        if (Status == Status.Active)
            return new ResultError("A deleted schedule cannot be activated.", ErrorType.Validation);

        if (_courts.Count == 0)
            return new ResultError("A daily schedule must have at least one court before it can be activated.",
                ErrorType.Validation);

        if (_times.Min(st => st.TimeInterval.Start) <= DateTime.Now)
            return new ResultError("A daily schedule can only be activated if its date and time is in the future.",
                ErrorType.Validation);

        var scheduleDate = DateOnly.FromDateTime(_times.Min(st => st.TimeInterval.Start));
        if (conflictChecker.ActiveScheduleExistsOnDate(Id, scheduleDate))
            return new ResultError("Another active daily schedule already exists on this date.", ErrorType.Validation);

        Status = Status.Active;
        return None.Value;
    }

    public Result<None> Delete()
    {
        Status = Status.Deleted;
        return None.Value;
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
    
    internal void RemoveCourt(CourtId courtId)
    {
        _courts.Remove(courtId);
    }
    
    public Result<None> UpdateDate(DateTime newDate)
    {
        if (!Status.Equals(Status.Draft))
            return new ResultError("Schedule date can only be updated while in Draft status.", ErrorType.Validation);

        if (newDate.Date < DateTime.Today)
            return new ResultError("Schedule date cannot be set to a date in the past.", ErrorType.Validation);

        return Result.Success();
    }

    public Result<None> UpdateTimes(TimeInterval timeInterval)
    {
        if (!Status.Equals(Status.Draft))
            return new ResultError("Schedule time intervals can only be updated while in Draft status.",
                ErrorType.Validation);

        // Rebuild first element with new start, keeping its original end
        var firstOriginal =  _times[0].TimeInterval;
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

    public Result<None> MarkVipTimeSpan(TimeInterval vipInterval)
    {
        var validation = Result.Combine(
            ValidateIsDraft(),
            ValidateVipMinDuration(vipInterval),
            ValidateVipWithinSchedule(vipInterval)
        );

        if (validation is Result<None>.Failure f)
            return Result.Failure<None>(f.Errors);

        RebuildTimeSlotsWithVip(vipInterval);
        return Result.Success();
    }

    private Result<None> ValidateIsDraft()
    {
        if (Status != Status.Draft)
            return Result.Failure("VIP time spans can only be set while schedule is in Draft status.", ErrorType.Validation);
        return Result.Success();
    }

    private static Result<None> ValidateVipMinDuration(TimeInterval vipInterval)
    {
        if (vipInterval.Duration < TimeSpan.FromMinutes(30))
            return Result.Failure("VIP time span must be at least 30 minutes.", ErrorType.Validation);
        return Result.Success();
    }

    private Result<None> ValidateVipWithinSchedule(TimeInterval vipInterval)
    {
        var scheduleStart = _times.Min(t => t.TimeInterval.Start);
        var scheduleEnd = _times.Max(t => t.TimeInterval.End);

        if (vipInterval.Start < scheduleStart || vipInterval.End > scheduleEnd)
            return Result.Failure("VIP time span must be within the schedule time boundaries.", ErrorType.Validation);
        return Result.Success();
    }

    private void RebuildTimeSlotsWithVip(TimeInterval newVip)
    {
        var scheduleStart = _times.Min(t => t.TimeInterval.Start);
        var scheduleEnd = _times.Max(t => t.TimeInterval.End);

        // Collect all VIP intervals (existing + new), then merge overlapping/adjacent
        var allVipIntervals = _times
            .Where(t => t.IsVip)
            .Select(t => (Start: t.TimeInterval.Start, End: t.TimeInterval.End))
            .Append((Start: newVip.Start, End: newVip.End))
            .OrderBy(v => v.Start)
            .ToList();

        var merged = MergeIntervals(allVipIntervals);

        // Rebuild _times: fill schedule with regular slots, inserting VIP where needed
        var newSlots = new List<ScheduleTimeInterval>();
        var cursor = scheduleStart;

        foreach (var vip in merged)
        {
            // Regular gap before this VIP
            if (vip.Start > cursor)
            {
                var regular = ((Result<TimeInterval>.Success)TimeInterval.Create(cursor, vip.Start)).Value;
                newSlots.Add(((Result<ScheduleTimeInterval>.Success)ScheduleTimeInterval.Create(regular, false)).Value);
            }

            // VIP slot
            var vipTi = ((Result<TimeInterval>.Success)TimeInterval.Create(vip.Start, vip.End)).Value;
            newSlots.Add(((Result<ScheduleTimeInterval>.Success)ScheduleTimeInterval.Create(vipTi, true)).Value);

            cursor = vip.End;
        }

        // Regular gap after last VIP
        if (cursor < scheduleEnd)
        {
            var regular = ((Result<TimeInterval>.Success)TimeInterval.Create(cursor, scheduleEnd)).Value;
            newSlots.Add(((Result<ScheduleTimeInterval>.Success)ScheduleTimeInterval.Create(regular, false)).Value);
        }

        _times = newSlots;
    }

    private static List<(DateTime Start, DateTime End)> MergeIntervals(List<(DateTime Start, DateTime End)> intervals)
    {
        if (intervals.Count == 0) return [];

        var result = new List<(DateTime Start, DateTime End)> { intervals[0] };

        for (var i = 1; i < intervals.Count; i++)
        {
            var last = result[^1];
            var current = intervals[i];

            // Overlapping or adjacent — merge
            if (current.Start <= last.End)
            {
                result[^1] = (last.Start, current.End > last.End ? current.End : last.End);
            }
            else
            {
                result.Add(current);
            }
        }

        return result;
    }
}