using VIAPadelClub.Core.Domain.Aggregates.Court;
using VIAPadelClub.Core.Domain.Aggregates.Schedule;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Domain.Services;

public class DeleteScheduleService
{
    public static Result<IReadOnlyList<ViaEmail>> Handle(Schedule schedule, IReadOnlyList<Court> courts,
        DateTime currentTime)
    {
        var deleteResult = schedule.Delete(currentTime);
        if (deleteResult is Result<None>.Failure f)
            return Result.Failure<IReadOnlyList<ViaEmail>>(f.Errors);

        // Cancel all future bookings on courts in this schedule
        var affectedEmails = new List<ViaEmail>();
        foreach (var court in courts)
        {
            var bookingsToCancel = court.Bookings
                .Where(b => !b.IsCancelled && b.ScheduleId == schedule.Id && b.TimeInterval.Start > currentTime)
                .ToList();

            foreach (var booking in bookingsToCancel)
            {
                court.ForceCancelBooking(booking.Id);
                affectedEmails.Add(booking.PlayerEmail);
            }
        }

        return Result.Success<IReadOnlyList<ViaEmail>>(affectedEmails.AsReadOnly());
    }
}