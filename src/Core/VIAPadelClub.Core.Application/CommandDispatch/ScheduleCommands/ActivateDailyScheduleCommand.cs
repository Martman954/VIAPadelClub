using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.CommandDispatch.ScheduleCommands;

public class ActivateDailyScheduleCommand
{
    public Guid ScheduleId { get; set; }

    private ActivateDailyScheduleCommand(Guid scheduleId)
    {
        ScheduleId = scheduleId;
    }

    public static Result<ActivateDailyScheduleCommand> Create(Guid scheduleId)
    {
        
        return new ActivateDailyScheduleCommand(scheduleId);
    }
}