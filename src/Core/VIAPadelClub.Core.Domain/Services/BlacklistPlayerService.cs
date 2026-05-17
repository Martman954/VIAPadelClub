using VIAPadelClub.Core.Domain.Aggregates.Player;
using VIAPadelClub.Core.Domain.Contracts.Player;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Services;

public class BlacklistPlayerService
{
    public static Result<None> Handle(Player player, IPlayerBookingFinder bookingFinder)
    {
        if (player.isBlackListed)
            return Result.Failure("Player is already blacklisted.", ErrorType.Validation);
        
        var courts = bookingFinder.GetCourtsWithBookingsForPlayer(player.Email);
        foreach (var court in courts)
        {
            var bookingsToCancel = court.Bookings
                .Where(b => !b.IsCancelled && b.PlayerEmail == player.Email)
                .ToList();

            foreach (var booking in bookingsToCancel)
            {
                court.ForceCancelBooking(booking.Id);
            }
        }
        
        player.Blacklist();

        return Result.Success();
    }
}

