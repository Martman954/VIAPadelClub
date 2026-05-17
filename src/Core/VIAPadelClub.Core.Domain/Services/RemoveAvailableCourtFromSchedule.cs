using VIAPadelClub.Core.Domain.Aggregates.Court;
using VIAPadelClub.Core.Domain.Aggregates.Schedule;
using VIAPadelClub.Core.Domain.Aggregates.Schedule.Enums;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Services;

public class RemoveAvailableCourtFromSchedule
{
    // TODO: Make sure to collect errors in array before returning an error
    public static Result<IReadOnlyList<ViaEmail>> Handle(Schedule schedule, Court court, DateTime currentTime)
    {
        var courtId = court.Id;
        
        if (!schedule.Courts.Contains(courtId))
            return Result.Failure<IReadOnlyList<ViaEmail>>(
                new ResultError("The court was not found in the daily schedule.", ErrorType.NotFound));

        if (schedule.Status == Status.Draft)
        {
            schedule.RemoveCourt(courtId);
            return Result.Success<IReadOnlyList<ViaEmail>>(new List<ViaEmail>());
        }

        var scheduleDate = schedule.Times[0].TimeInterval.Start.Date;

        if (currentTime.Date > scheduleDate)
            return Result.Failure<IReadOnlyList<ViaEmail>>(
                new ResultError("Court from a past schedule cannot be removed.", ErrorType.Validation));

        var activeBookings = court.Bookings
            .Where(b => !b.IsCancelled && b.ScheduleId == schedule.Id)
            .ToList();
        
        if (currentTime.Date == scheduleDate)
        {
            var upcomingBookings = activeBookings
                .Where(b => b.TimeInterval.Start > currentTime)
                .ToList();
            
            if (upcomingBookings.Count != 0)
                return Result.Failure<IReadOnlyList<ViaEmail>>(
                    new ResultError("Cannot remove court while there are upcoming bookings on the same day.", ErrorType.Validation));

            // All games have been played — remove court, no emails
            schedule.RemoveCourt(courtId);
            return Result.Success<IReadOnlyList<ViaEmail>>(new List<ViaEmail>());
        }

        // Before schedule date - cancel all bookings and send emails
        var affectedEmails = new List<ViaEmail>();
        foreach (var booking in activeBookings)
        {
            court.CancelBooking(booking.Id, currentTime);
            affectedEmails.Add(booking.PlayerEmail);
        }

        schedule.RemoveCourt(courtId);

        return Result.Success<IReadOnlyList<ViaEmail>>(affectedEmails.AsReadOnly());
    }
}