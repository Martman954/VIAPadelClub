using VIAPadelClub.Core.Domain.Aggregates.Player;
using VIAPadelClub.Core.Domain.Contracts.Player;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Domain.Services;

public class QuarantinePlayerService
{
    public static Result<None> Handle(Player player, DateTime currentTime, IPlayerBookingFinder bookingFinder)
    {
        var quarantineResult = player.QuarantinePlayer(currentTime);
        if (quarantineResult is Result<None>.Failure f)
            return Result.Failure<None>(f.Errors);

        var quarantine = player.Quarantine!;
        var courts = bookingFinder.GetCourtsWithBookingsForPlayer(player.Email);

        foreach (var court in courts)
        {
            var bookingsToCancel = court.Bookings
                .Where(b => !b.IsCancelled
                            && b.PlayerEmail == player.Email
                            && quarantine.CoversDate(b.TimeInterval.Start))
                .ToList();

            foreach (var booking in bookingsToCancel)
            {
                court.ForceCancelBooking(booking.Id);
            }
        }

        return Result.Success();
    }
}

