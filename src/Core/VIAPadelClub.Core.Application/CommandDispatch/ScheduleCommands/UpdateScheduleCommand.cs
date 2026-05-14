using VIAPadelClub.Core.Domain.Aggregates.Schedule.Enums;
using VIAPadelClub.Core.Domain.Aggregates.Schedule.ValueObjects;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace Features.CommandDispatch.ScheduleCommands;

public class UpdateScheduleCommand
{
    // Manager Updates Time and date on daily schedule
    public Guid ScheduleId { get;}
    public ScheduleTimeInterval ScheduleTimeInterval { get; }
    
    private UpdateScheduleCommand(Guid scheduleId ,ScheduleTimeInterval scheduleTimeInterval)
    {
        ScheduleId = scheduleId;
        ScheduleTimeInterval = scheduleTimeInterval;
    }

    public static Result<UpdateScheduleCommand> Create(Guid scheduleId, ScheduleTimeInterval scheduleTimeInterval)
    {
        return new UpdateScheduleCommand(scheduleId, scheduleTimeInterval);
    }
}