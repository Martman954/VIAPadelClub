using VIAPadelClub.Core.Domain.Aggregates.Schedules.Enums;
using VIAPadelClub.Core.Domain.Aggregates.Schedules.ValueObjects;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Schedules;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Aggregates.Schedules;

public sealed class Schedule : AggregateRoot<ScheduleId>
{
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

    private Schedule(ScheduleId id, ScheduleTimeInterval defaultTime)
    {
        Id = id;
        Status = Status.Draft;
        _courts = [];
        _times = [defaultTime];
    }

    /// <summary>For EF Core use only.</summary>
    private Schedule()
    {
        _times  = [];
        _courts = [];
    }

    public static Result<Schedule> Create()
    {
        var today = DateTime.Today;
        var start = today.Add(DefaultStart.ToTimeSpan());
        var end = today.Add(DefaultEnd.ToTimeSpan());
        var timeIntervalResult = TimeInterval.Create(start, end);

        if (timeIntervalResult is Result<TimeInterval>.Failure f1)
            return Result.Failure<Schedule>(f1.Errors);

        var scheduleTimeIntervalResult = ScheduleTimeInterval.Create(timeIntervalResult.Payload, false);

        if (scheduleTimeIntervalResult is Result<ScheduleTimeInterval>.Failure f2)
            return Result.Failure<Schedule>(f2.Errors);

        return new Schedule(ScheduleId.New(), scheduleTimeIntervalResult.Payload);
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

    internal Result<None> Delete(DateTime currentTime)
    {
        if (Status == Status.Deleted)
            return Result.Failure("Schedule is already deleted.", ErrorType.Validation);

        var scheduleDate = _times.Min(t => t.TimeInterval.Start).Date;

        if (currentTime.Date > scheduleDate)
            return Result.Failure("Past schedules cannot be deleted.", ErrorType.Validation);

        if (currentTime.Date == scheduleDate)
            return Result.Failure("A daily schedule cannot be deleted on the same date as the schedule.", ErrorType.Validation);

        Status = Status.Deleted;
        _courts.Clear();
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

    internal void RemoveCourt(CourtId courtId)
    {
        _courts.Remove(courtId);
    }

    public Result<None> UpdateDate(DateTime newDate)
    {
        if (Status != Status.Draft)
            return new ResultError(
                "Schedule date can only be updated while in Draft status.",
                ErrorType.Validation);

        if (newDate.Date < DateTime.Today)
            return new ResultError(
                "Schedule date cannot be set to a date in the past.",
                ErrorType.Validation);

        _times = _times
            .Select(slot =>
            {
                var startTime = slot.TimeInterval.Start.TimeOfDay;
                var endTime = slot.TimeInterval.End.TimeOfDay;

                var newStart = newDate.Date.Add(startTime);
                var newEnd = newDate.Date.Add(endTime);

                var interval = TimeInterval.Create(newStart, newEnd).Payload;

                return ScheduleTimeInterval
                    .Create(interval, slot.IsVip)
                    .Payload;
            })
            .ToList();

        return Result.Success();
    }

    public Result<None> UpdateTimes(TimeInterval timeInterval)
    {
        if (!Status.Equals(Status.Draft))
            return new ResultError("Schedule time intervals can only be updated while in Draft status.",
                ErrorType.Validation);

        var newFirstTimeSlot = RebuildSlot(_times[0], newStart: timeInterval.Start);
        if (newFirstTimeSlot is Result<ScheduleTimeInterval>.Failure f1)
            return Result.Failure<None>(f1.Errors);

        var newLastTimeSlot = RebuildSlot(_times[^1], newEnd: timeInterval.End);
        if (newLastTimeSlot is Result<ScheduleTimeInterval>.Failure f2)
            return Result.Failure<None>(f2.Errors);

        _times[0] = newFirstTimeSlot.Payload;
        _times[^1] = newLastTimeSlot.Payload;

        return Result.Success();
    }

    private static Result<ScheduleTimeInterval> RebuildSlot(ScheduleTimeInterval slot, DateTime? newStart = null, DateTime? newEnd = null)
    {
        var start = newStart ?? slot.TimeInterval.Start;
        var end = newEnd ?? slot.TimeInterval.End;

        var intervalResult = TimeInterval.Create(start, end);
        if (intervalResult is Result<TimeInterval>.Failure f)
            return Result.Failure<ScheduleTimeInterval>(f.Errors);

        return ScheduleTimeInterval.Create(intervalResult.Payload, slot.IsVip);
    }

    public Result<None> MarkVipTimeSpan(TimeInterval vipInterval, INonVipBookingOverlapChecker overlapChecker)
    {
        var validation = Result.Combine(
            ValidateIsDraft(),
            ValidateVipMinDuration(vipInterval),
            ValidateVipTimeFormat(vipInterval),
            ValidateVipWithinSchedule(vipInterval),
            ValidateNoNonVipBookingsOverlap(vipInterval, overlapChecker)
        );

        if (validation is Result<None>.Failure f)
            return Result.Failure<None>(f.Errors);

        RebuildTimeSlotsWithVip(vipInterval);
        return Result.Success();
    }

    private Result<None> ValidateIsDraft()
    {
        if (Status != Status.Draft)
            return Result.Failure("VIP time spans can only be set while schedule is in Draft status.",
                ErrorType.Validation);
        return Result.Success();
    }

    private static Result<None> ValidateVipMinDuration(TimeInterval vipInterval)
    {
        if (vipInterval.Duration < TimeSpan.FromMinutes(30))
            return Result.Failure("VIP time span must be at least 30 minutes.", ErrorType.Validation);
        return Result.Success();
    }

    private static Result<None> ValidateVipTimeFormat(TimeInterval vipInterval)
    {
        if (vipInterval.Start.Minute is not (0 or 30) || vipInterval.End.Minute is not (0 or 30))
            return Result.Failure("VIP time span start and end must be on the hour (:00) or half hour (:30).", ErrorType.Validation);
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

    private Result<None> ValidateNoNonVipBookingsOverlap(TimeInterval vipInterval, INonVipBookingOverlapChecker checker)
    {
        if (checker.HasNonVipBookingsInTimeSpan(Id, vipInterval))
            return Result.Failure("The chosen VIP time span overlaps with existing bookings by non-VIP players.",
                ErrorType.Validation);
        return Result.Success();
    }

    private void RebuildTimeSlotsWithVip(TimeInterval newVip)
    {
        var scheduleStart = _times.Min(t => t.TimeInterval.Start);
        var scheduleEnd = _times.Max(t => t.TimeInterval.End);

        var allVipIntervals = _times
            .Where(t => t.IsVip)
            .Select(t => (t.TimeInterval.Start, t.TimeInterval.End))
            .Append((newVip.Start, newVip.End))
            .OrderBy(v => v.Start)
            .ToList();

        var merged = MergeIntervals(allVipIntervals);
        var newSlots = new List<ScheduleTimeInterval>();
        var cursor = scheduleStart;

        foreach (var vip in merged)
        {
            if (vip.Start > cursor)
            {
                var regular = TimeInterval.Create(cursor, vip.Start).Payload;
                newSlots.Add(ScheduleTimeInterval.Create(regular, false).Payload);
            }

            var vipTimeInterval = TimeInterval.Create(vip.Start, vip.End).Payload;
            newSlots.Add(ScheduleTimeInterval.Create(vipTimeInterval, true).Payload);

            cursor = vip.End;
        }

        if (cursor < scheduleEnd)
        {
            var regular = TimeInterval.Create(cursor, scheduleEnd).Payload;
            newSlots.Add(ScheduleTimeInterval.Create(regular, false).Payload);
        }

        _times = newSlots;
    }

    /// <summary>
    /// Creates a merged list of intervals, where overlapping intervals are combined into one
    /// </summary>
    private static List<(DateTime Start, DateTime End)> MergeIntervals(List<(DateTime Start, DateTime End)> intervals)
    {
        if (intervals.Count == 0) return [];

        var result = new List<(DateTime Start, DateTime End)> { intervals[0] };

        for (var i = 1; i < intervals.Count; i++)
        {
            var last = result[^1];
            var current = intervals[i];

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