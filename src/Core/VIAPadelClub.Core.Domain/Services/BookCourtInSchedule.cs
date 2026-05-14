using VIAPadelClub.Core.Domain.Aggregates.Court;
using VIAPadelClub.Core.Domain.Aggregates.Player;
using VIAPadelClub.Core.Domain.Aggregates.Schedule;
using VIAPadelClub.Core.Domain.Aggregates.Schedule.Enums;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Court;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Services;

public class BookCourtInSchedule(ICourtHasBookingChecker bookingChecker)
{
    public Result<BookingId> Handle(BookingRequest request, DateTime currentTime)
    {
        var (player, court, schedule, timeInterval) = request;

        var validation = Result.Combine(
            ValidateScheduleIsActive(schedule),
            ValidateCourtInSchedule(schedule, court.Id),
            ValidatePlayerNotBlacklisted(player),
            ValidatePlayerHasNoBookingOnDate(player.Email, timeInterval.Start.Date),
            court.ValidateBooking(timeInterval, schedule, player, currentTime)
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

    private Result<None> ValidatePlayerHasNoBookingOnDate(ViaEmail email, DateTime date)
    {
        if (bookingChecker.HasBooking(email, date))
            return Result.Failure("A player can have a maximum of one booking per day.", ErrorType.Validation);
        return Result.Success();
    }
}