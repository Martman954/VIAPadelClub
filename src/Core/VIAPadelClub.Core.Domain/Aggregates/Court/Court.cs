using VIAPadelClub.Core.Domain.Aggregates.Court.Entities;
using VIAPadelClub.Core.Domain.Aggregates.Schedule.Enums;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Aggregates.Court;

public sealed class Court
{
    public CourtId Id { get; }

    private readonly List<Booking> _bookings = [];
    public IReadOnlyList<Booking> Bookings => _bookings.AsReadOnly();

    private Court(CourtId id)
    {
        Id = id;
    }

    public static Result<Court> Create(string courtId)
    {
        return CourtId.Create(courtId) switch
        {
            Result<CourtId>.Success s => new Court(s.Value),
            Result<CourtId>.Failure f => Result.Failure<Court>(f.Errors),
            _ => throw new InvalidOperationException()
        };
    }

    public Result<BookingId> Book(ViaEmail email, TimeInterval timeInterval, CourtId courtId, Schedule.Schedule schedule, Player.Player player, DateTime currentTime)
    {
        return Result.Combine(
            ValidateSchedule(schedule, courtId),
            ValidateTimeFormat(timeInterval),
            ValidateDuration(timeInterval),
            ValidateWithinSchedule(timeInterval, schedule),
            ValidateNotInPast(timeInterval, currentTime),
            ValidatePlayer(player, email, timeInterval),
            ValidateVipAccess(player, schedule, timeInterval),
            ValidateNoOverlap(timeInterval),
            ValidateNoHoles(timeInterval, schedule)
        ).WithSuccessPayload(CreateBooking(timeInterval, schedule.Id, email));
    }

    private Result<None> ValidateSchedule(Schedule.Schedule schedule, CourtId courtId) =>
        Result.Combine(
            schedule.Status == Status.Active
                ? Result.Success()
                : Result.Failure("Courts cannot be booked if the Daily Schedule is not active.", ErrorType.Validation),
            schedule.Courts.Contains(courtId)
                ? Result.Success()
                : Result.Failure("The court was not found in the daily schedule.", ErrorType.NotFound)
        );

    private Result<None> ValidateTimeFormat(TimeInterval t) =>
        (t.Start.Minute % 30 == 0 && t.End.Minute % 30 == 0)
            ? Result.Success()
            : Result.Failure("Booking start and end time minutes must be 00 or 30.", ErrorType.Validation);

    private Result<None> ValidateDuration(TimeInterval t) =>
        t.Duration < TimeSpan.FromHours(1)
            ? Result.Failure("A booking must be at least one hour.", ErrorType.Validation)
            : t.Duration > TimeSpan.FromHours(3)
                ? Result.Failure("A booking must be at most three hours.", ErrorType.Validation)
                : Result.Success();

    private Result<None> ValidateWithinSchedule(TimeInterval t, Schedule.Schedule s) =>
        Result.Combine(
            t.Start >= s.Times.TimeInterval.Start
                ? Result.Success()
                : Result.Failure("Booking starts before schedule.", ErrorType.Validation),
            t.End <= s.Times.TimeInterval.End
                ? Result.Success()
                : Result.Failure("Booking ends after schedule.", ErrorType.Validation)
        );

    private Result<None> ValidateNotInPast(TimeInterval t, DateTime now) =>
        t.Start >= now
            ? Result.Success()
            : Result.Failure("A booking cannot start in the past.", ErrorType.Validation);

    private Result<None> ValidatePlayer(Player.Player player, ViaEmail email, TimeInterval t) =>
        Result.Combine(
            !player.isBlackListed
                ? Result.Success()
                : Result.Failure("Blacklisted players cannot book courts.", ErrorType.Validation),
            !_bookings.Any(b => !b.IsCancelled && b.PlayerEmail == email && b.TimeInterval.Start.Date == t.Start.Date)
                ? Result.Success()
                : Result.Failure("A player can have a maximum of one booking per day.", ErrorType.Validation)
        );

    private Result<None> ValidateVipAccess(Player.Player player, Schedule.Schedule schedule, TimeInterval t) =>
        player.VipStatus != null || !schedule.ActiveTime.Where(s => s.IsVip).Any(s => Overlaps(t, s.TimeInterval))
            ? Result.Success()
            : Result.Failure("Non-VIP players cannot book during VIP time.", ErrorType.Validation);

    private Result<None> ValidateNoOverlap(TimeInterval t) =>
        _bookings.Where(b => !b.IsCancelled).Any(b => Overlaps(t, b.TimeInterval))
            ? Result.Failure("The court is not available in the selected time span.", ErrorType.Validation)
            : Result.Success();

    private Result<None> ValidateNoHoles(TimeInterval t, Schedule.Schedule s)
    {
        var oneHour = TimeSpan.FromHours(1);
        var active = _bookings.Where(b => !b.IsCancelled).ToList();

        bool LeavesSmallGap(TimeSpan gap) => gap > TimeSpan.Zero && gap < oneHour;

        var boundaries = active.SelectMany(b => new[] { b.TimeInterval.Start, b.TimeInterval.End })
            .Append(s.Times.TimeInterval.Start)
            .Append(s.Times.TimeInterval.End)
            .Distinct().OrderBy(x => x).ToList();

        foreach (var point in boundaries)
        {
            if (LeavesSmallGap(t.Start - point) || LeavesSmallGap(point - t.End))
                return Result.Failure("A booking may not leave gaps less than one hour.", ErrorType.Validation);
        }

        return Result.Success();
    }

    private BookingId CreateBooking(TimeInterval t, Guid scheduleId, ViaEmail email)
    {
        var id = BookingId.New();
        _bookings.Add(new Booking(id, t, scheduleId, email));
        return id;
    }

    private static bool Overlaps(TimeInterval a, TimeInterval b) =>
        a.Start < b.End && a.End > b.Start;

    public Result<None> CancelBooking(BookingId bookingId)
    {
        var booking = _bookings.FirstOrDefault(b => b.Id == bookingId);

        if (booking is null)
        {
            return Result.Failure<None>(
                new ResultError("Booking not found.", ErrorType.NotFound));
        }

        if (booking.IsCancelled)
        {
            return Result.Failure<None>(
                new ResultError("Booking is already cancelled.", ErrorType.Validation));
        }

        var now = DateTime.Now;
        var start = booking.TimeInterval.Start;

        if (start <= now)
        {
            return Result.Failure<None>(
                new ResultError("Past bookings cannot be cancelled.", ErrorType.Validation));
        }

        var sameDay = now.Date == start.Date;
        var lessThanOneHour = (start - now) < TimeSpan.FromHours(1);

        if (sameDay && lessThanOneHour)
        {
            return Result.Failure<None>(
                new ResultError("Booking cannot be cancelled less than one hour before start time.",
                    ErrorType.Validation));
        }

        booking.Cancel();

        return Result.Success();
    }
}