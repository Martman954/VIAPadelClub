using VIAPadelClub.Core.Domain.Aggregates.Court.Entities;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.CommandDispatch.PlayerCommands;

public class PlayerMakesABookingCommand
{
    public Booking Booking { get; }
    public CourtId CourtId { get; }


    private PlayerMakesABookingCommand(Booking booking, CourtId courtId)
    {
        Booking = booking;
        CourtId = courtId;
    }

    public Result<PlayerMakesABookingCommand> Create(Booking booking, CourtId courtId)
    {
        
        return new PlayerMakesABookingCommand(booking, courtId);
    }

}