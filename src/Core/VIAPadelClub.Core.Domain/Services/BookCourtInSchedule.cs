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
            ValidateVipAccess(player, schedule, timeInterval)
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
}