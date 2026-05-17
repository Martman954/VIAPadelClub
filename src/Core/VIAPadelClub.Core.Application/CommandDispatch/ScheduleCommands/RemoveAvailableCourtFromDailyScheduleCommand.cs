using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.CommandDispatch.CourtCommands;

public class RemoveAvailableCourtFromDailyScheduleCommand
{
    public CourtId CourtId { get; }
    public Guid ScheduleId { get; }

    private RemoveAvailableCourtFromDailyScheduleCommand(CourtId courtId, Guid scheduleId)
    {
        CourtId = courtId;
        ScheduleId = scheduleId;
    }

    private static Result<RemoveAvailableCourtFromDailyScheduleCommand> Create(CourtId courtId, Guid scheduleId)
    {
        return new RemoveAvailableCourtFromDailyScheduleCommand(courtId, scheduleId);
    }
}