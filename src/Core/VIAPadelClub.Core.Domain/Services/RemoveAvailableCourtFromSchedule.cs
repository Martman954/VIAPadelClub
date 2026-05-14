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
                new ResultError("Court can only be removed on or before the date of the schedule.", ErrorType.Validation));

        var activeBookings = court.Bookings
            .Where(b => !b.IsCancelled && b.ScheduleId == schedule.Id)
            .ToList();

        // Only cancel bookings that haven't started yet; past bookings are left as-is
        var futureBookings = activeBookings
            .Where(b => b.TimeInterval.Start > currentTime)
            .ToList();

        var affectedEmails = new List<ViaEmail>();
        foreach (var booking in futureBookings)
        {
            court.CancelBooking(booking.Id);
            affectedEmails.Add(booking.PlayerEmail);
        }

        schedule.RemoveCourt(courtId);

        return Result.Success<IReadOnlyList<ViaEmail>>(affectedEmails.AsReadOnly());
    }
}