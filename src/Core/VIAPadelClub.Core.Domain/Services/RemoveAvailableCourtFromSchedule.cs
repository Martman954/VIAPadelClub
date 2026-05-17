using VIAPadelClub.Core.Domain.Aggregates.Court;
using VIAPadelClub.Core.Domain.Aggregates.Schedule;
using VIAPadelClub.Core.Domain.Aggregates.Schedule.Enums;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Services;

public class RemoveAvailableCourtFromSchedule
{
    public static Result<IReadOnlyList<ViaEmail>> Handle(Schedule schedule, Court court, DateTime currentTime)
    {
        var courtId = court.Id;

        var validation = Result.Combine(
            ValidateCourtInSchedule(schedule, courtId),
            ValidateNotPastSchedule(schedule, currentTime),
            ValidateNoUpcomingBookingsSameDay(court, schedule, currentTime)
        );

        if (validation is Result<None>.Failure f)
            return Result.Failure<IReadOnlyList<ViaEmail>>(f.Errors);

        // Draft or same-day with all games played - just remove, no emails
        if (schedule.Status == Status.Draft || currentTime.Date == GetScheduleDate(schedule))
        {
            schedule.RemoveCourt(courtId);
            return Result.Success<IReadOnlyList<ViaEmail>>(new List<ViaEmail>());
        }

        // Before schedule date - cancel all bookings and collect emails
        var affectedEmails = CancelActiveBookings(court, schedule, currentTime);
        schedule.RemoveCourt(courtId);

        return Result.Success(affectedEmails);
    }

    private static Result<None> ValidateCourtInSchedule(Schedule schedule, CourtId courtId)
    {
        if (!schedule.Courts.Contains(courtId))
            return Result.Failure("The court was not found in the daily schedule.", ErrorType.NotFound);
        return Result.Success();
    }

    private static Result<None> ValidateNotPastSchedule(Schedule schedule, DateTime currentTime)
    {
        if (currentTime.Date > GetScheduleDate(schedule))
            return Result.Failure("Court from a past schedule cannot be removed.", ErrorType.Validation);
        return Result.Success();
    }

    private static Result<None> ValidateNoUpcomingBookingsSameDay(Court court, Schedule schedule, DateTime currentTime)
    {
        var scheduleDate = GetScheduleDate(schedule);

        if (currentTime.Date != scheduleDate)
            return Result.Success();

        var hasUpcoming = court.Bookings
            .Any(b => !b.IsCancelled && b.ScheduleId == schedule.Id && b.TimeInterval.Start > currentTime);

        if (hasUpcoming)
            return Result.Failure("Cannot remove court while there are upcoming bookings on the same day.", ErrorType.Validation);

        return Result.Success();
    }

    private static IReadOnlyList<ViaEmail> CancelActiveBookings(Court court, Schedule schedule, DateTime currentTime)
    {
        var activeBookings = court.Bookings
            .Where(b => !b.IsCancelled && b.ScheduleId == schedule.Id)
            .ToList();

        var emails = new List<ViaEmail>();
        foreach (var booking in activeBookings)
        {
            court.CancelBooking(booking.Id, currentTime);
            emails.Add(booking.PlayerEmail);
        }

        return emails.AsReadOnly();
    }

    private static DateTime GetScheduleDate(Schedule schedule)
        => schedule.Times[0].TimeInterval.Start.Date;
}