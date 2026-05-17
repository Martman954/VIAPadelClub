using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.CommandDispatch.ScheduleCommands;

public class SetPartOfDailyScheduleAsVIPOnlyCommand
{
    public Guid ScheduleId { get; }
    public TimeInterval Interval { get; }
    
    private SetPartOfDailyScheduleAsVIPOnlyCommand(Guid scheduleId, TimeInterval interval)
    {
        ScheduleId = scheduleId;
        Interval = interval;
    }

    public static Result<SetPartOfDailyScheduleAsVIPOnlyCommand> Create(Guid scheduleId, TimeInterval duration)
    {
        return new SetPartOfDailyScheduleAsVIPOnlyCommand(scheduleId, duration);
    }
}