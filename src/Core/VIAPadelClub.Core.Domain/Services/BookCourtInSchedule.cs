using VIAPadelClub.Core.Domain.Aggregates.Court;
using VIAPadelClub.Core.Domain.Aggregates.Player;
using VIAPadelClub.Core.Domain.Aggregates.Schedule;
using VIAPadelClub.Core.Domain.Aggregates.Schedule.Enums;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Court;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Services;

public class BookCourtInSchedule()
{
    public static Result<BookingId> Handle(BookingRequest request, DateTime currentTime, ICourtHasBookingChecker bookingChecker)
    {
        var (player, court, schedule, timeInterval) = request;

        var validation = Result.Combine(
            ValidateScheduleIsActive(schedule),
            ValidateCourtInSchedule(schedule, court.Id),
            ValidatePlayerNotBlacklisted(player),
            ValidatePlayerHasNoBookingOnDate(player.Email, timeInterval.Start.Date, bookingChecker),
            ValidateBookingTimeFormat(timeInterval),
            ValidateNotInPast(timeInterval, currentTime),
            ValidateWithinSchedule(timeInterval, schedule),
            ValidateVipAccess(player, schedule, timeInterval),
            ValidateNoOverlap(court, timeInterval),
            ValidateNoSmallGaps(court, schedule, timeInterval)
        );

        if (validation is Result<None>.Failure f)
            return Result.Failure<BookingId>(f.Errors);

        return court.AddBooking(timeInterval, schedule.Id, player.Email);
    }

    private static Result<None> ValidateScheduleIsActive(Schedule schedule)
    {
        if (schedule.Status != Status.Active)
            return Result.Failure("Courts cannot be booked if the Daily Schedule is not active.", ErrorType.Validation);
        return Result.Success();
    }

    private static Result<None> ValidateCourtInSchedule(Schedule schedule, CourtId courtId)
    {
        if (!schedule.Courts.Contains(courtId))
            return Result.Failure("The court was not found in the daily schedule.", ErrorType.NotFound);
        return Result.Success();
    }

    private static Result<None> ValidatePlayerNotBlacklisted(Player player)
    {
        if (player.isBlackListed)
            return Result.Failure("Blacklisted players cannot book courts.", ErrorType.Validation);
        return Result.Success();
    }

    private static Result<None> ValidatePlayerHasNoBookingOnDate(ViaEmail email, DateTime date,  ICourtHasBookingChecker bookingChecker)
    {
        if (bookingChecker.HasBooking(email, date))
            return Result.Failure("A player can have a maximum of one booking per day.", ErrorType.Validation);
        return Result.Success();
    }

    private static Result<None> ValidateNotInPast(TimeInterval timeInterval, DateTime currentTime)
    {
        if (timeInterval.Start < currentTime)
            return Result.Failure("A booking cannot start in the past.", ErrorType.Validation);
        return Result.Success();
    }

    private static Result<None> ValidateWithinSchedule(TimeInterval timeInterval, Schedule schedule)
    {
        var scheduleStart = schedule.Times.Min(st => st.TimeInterval.Start);
        var scheduleEnd = schedule.Times.Max(st => st.TimeInterval.End);

        var errors = new List<ResultError>();

        if (timeInterval.Start < scheduleStart)
            errors.Add(new ResultError("Booking starts before the schedule.", ErrorType.Validation));

        if (timeInterval.End > scheduleEnd)
            errors.Add(new ResultError("Booking ends after the schedule.", ErrorType.Validation));

        return errors.Count > 0
            ? Result.Failure<None>(errors.ToArray())
            : Result.Success();
    }

    private static Result<None> ValidateVipAccess(Player player, Schedule schedule, TimeInterval timeInterval)
    {
        if (player.VipStatus != null)
            return Result.Success();

        var overlapsVipSlot = schedule.VipTimes
            .Any(vip => timeInterval.Start < vip.TimeInterval.End && timeInterval.End > vip.TimeInterval.Start);

        if (overlapsVipSlot)
            return Result.Failure("Non-VIP players cannot book during VIP-only time slots.", ErrorType.Validation);

        return Result.Success();
    }

    private static Result<None> ValidateBookingTimeFormat(TimeInterval timeInterval)
    {
        var errors = new List<ResultError>();

        if (timeInterval.Start.Minute % 30 != 0)
            errors.Add(new ResultError("A booking must start on a whole or half hour (e.g. 14:00, 14:30).", ErrorType.Validation));

        if (timeInterval.End.Minute % 30 != 0)
            errors.Add(new ResultError("A booking must end on a whole or half hour (e.g. 16:00, 16:30).", ErrorType.Validation));

        var duration = timeInterval.End - timeInterval.Start;

        if (duration < TimeSpan.FromHours(1))
            errors.Add(new ResultError("A booking must be at least 1 hour.", ErrorType.Validation));

        if (duration > TimeSpan.FromHours(3))
            errors.Add(new ResultError("A booking must be at most 3 hours.", ErrorType.Validation));

        if (duration.Minutes % 30 != 0)
            errors.Add(new ResultError("A booking duration must be in increments of 30 minutes.", ErrorType.Validation));

        return errors.Count > 0
            ? Result.Failure<None>(errors.ToArray())
            : Result.Success();
    }

    private static Result<None> ValidateNoOverlap(Court court, TimeInterval timeInterval)
    {
        var hasOverlap = court.Bookings
            .Where(b => !b.IsCancelled)
            .Any(b => timeInterval.Start < b.TimeInterval.End && timeInterval.End > b.TimeInterval.Start);

        if (hasOverlap)
            return Result.Failure("The court is not available in the selected time span.", ErrorType.Validation);

        return Result.Success();
    }

    private static Result<None> ValidateNoSmallGaps(Court court, Schedule schedule, TimeInterval timeInterval)
    {
        var oneHour = TimeSpan.FromHours(1);
        var activeBookings = court.Bookings.Where(b => !b.IsCancelled).ToList();

        // Collect all boundary points: schedule start/end + existing booking start/end
        var boundaries = schedule.Times
            .SelectMany(st => new[] { st.TimeInterval.Start, st.TimeInterval.End })
            .Concat(activeBookings.SelectMany(b => new DateTime[] { b.TimeInterval.Start, b.TimeInterval.End }))
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        foreach (var point in boundaries)
        {
            var gapBeforeStart = timeInterval.Start - point;
            var gapAfterEnd = point - timeInterval.End;

            if (gapBeforeStart > TimeSpan.Zero && gapBeforeStart < oneHour)
                return Result.Failure("A booking may not leave gaps less than one hour.", ErrorType.Validation);

            if (gapAfterEnd > TimeSpan.Zero && gapAfterEnd < oneHour)
                return Result.Failure("A booking may not leave gaps less than one hour.", ErrorType.Validation);
        }

        return Result.Success();
    }
}