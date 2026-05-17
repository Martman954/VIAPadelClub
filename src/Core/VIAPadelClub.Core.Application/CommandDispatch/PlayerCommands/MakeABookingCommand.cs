using VIAPadelClub.Core.Domain.Aggregates.Court.Entities;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.CommandDispatch.PlayerCommands;

public class MakeABookingCommand
{

    public CourtId CourtId { get; }

    public TimeInterval TimeInterval { get; }
    public Guid ScheduleId { get; }
    public ViaEmail ViaEmail { get; }
    
    private MakeABookingCommand(CourtId courtId, Guid scheduleId, TimeInterval timeInterval, ViaEmail viaEmail)
    {
        CourtId = courtId;
        TimeInterval = timeInterval;
        ScheduleId = scheduleId;
        ViaEmail = viaEmail;
    }

    public static Result<MakeABookingCommand> Create(CourtId courtId, Guid scheduleId, TimeInterval timeInterval, ViaEmail viaEmail)
    {
        return new MakeABookingCommand(courtId, scheduleId, timeInterval, viaEmail);
    }

}