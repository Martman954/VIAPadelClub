using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.CommandDispatch.PlayerCommands;

public class VIPMakeABookingCommand
{
    public CourtId CourtId { get; }

    public TimeInterval TimeInterval { get; }
    public Guid ScheduleId { get; }
    public ViaEmail ViaEmail { get; }
    
    private VIPMakeABookingCommand(CourtId courtId, Guid scheduleId, TimeInterval timeInterval, ViaEmail viaEmail)
    {
        CourtId = courtId;
        TimeInterval = timeInterval;
        ScheduleId = scheduleId;
        ViaEmail = viaEmail;
    }

    public static Result<VIPMakeABookingCommand> Create(CourtId courtId, Guid scheduleId, TimeInterval timeInterval, ViaEmail viaEmail)
    {
        return new VIPMakeABookingCommand(courtId, scheduleId, timeInterval, viaEmail);
    }
}