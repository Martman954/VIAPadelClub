using VIAPadelClub.Core.Domain.Aggregates.Schedule.Enums;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace Features.CommandDispatch.ScheduleCommands;

public class CreateScheduleCommand
{
    // ID, status(default: "draft"), List<Court>, Times(15:00-22:00), date->today
    public string Title { get; }
    public Status Status { get; }
    
    private CreateScheduleCommand(string title, Status status)
    {
        Title = title;
        Status = status;
    }

    public static Result<CreateScheduleCommand> Create(string title, Status status)
    {
        if(title.Length == 0)
            return new ResultError("Empty Title");
        
        return new CreateScheduleCommand(title, status);
    }
    
}