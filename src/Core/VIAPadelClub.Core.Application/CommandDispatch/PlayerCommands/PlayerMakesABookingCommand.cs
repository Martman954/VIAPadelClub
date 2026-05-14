using VIAPadelClub.Core.Domain.Aggregates.Court.Entities;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.CommandDispatch.PlayerCommands;

public class PlayerMakesABookingCommand
{

    public CourtId CourtId { get; }

    public TimeInterval TimeInterval { get; }
    public Guid ScheduleId { get; }
    public ViaEmail ViaEmail { get; }
    
    private PlayerMakesABookingCommand(CourtId courtId, Guid scheduleId, TimeInterval timeInterval, ViaEmail viaEmail)
    {
        CourtId = courtId;
        TimeInterval = timeInterval;
        ScheduleId = scheduleId;
        ViaEmail = viaEmail;
    }

    public Result<PlayerMakesABookingCommand> Create(CourtId courtId, Guid scheduleId, TimeInterval timeInterval, ViaEmail viaEmail)
    {
        return new PlayerMakesABookingCommand(courtId, scheduleId, timeInterval, viaEmail);
    }

}