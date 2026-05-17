using Features.CommandDispatch.PlayerCommands;
using VIAPadelClub.Core.Tools.OperationResult.Results;

public class CancelABookingCommand
{
    public Guid PlayerId { get; }
    public Guid ScheduleId { get; }
    public Guid BookingId { get; }

    private CancelABookingCommand(Guid playerId, Guid scheduleId, Guid bookingId)
    {
        PlayerId = playerId;
        BookingId = bookingId;
        ScheduleId = scheduleId;
    }


    public static Result<CancelABookingCommand> Create(Guid playerId, Guid scheduleId, Guid bookingId)
    {
        return new CancelABookingCommand(playerId, scheduleId, bookingId);
    }
}