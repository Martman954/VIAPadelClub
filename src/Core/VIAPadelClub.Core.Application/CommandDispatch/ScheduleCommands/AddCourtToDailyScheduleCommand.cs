using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace Features.CommandDispatch.ScheduleCommands;

public class AddCourtToDailyScheduleCommand
{
    public Guid ExistingScheduleId { get; }
    public string CourtId { get; }

    private AddCourtToDailyScheduleCommand(Guid existingScheduleId, string courtId)
    {
        ExistingScheduleId = existingScheduleId;
        CourtId = courtId;
    }

    public static Result<AddCourtToDailyScheduleCommand> Create(Guid existingScheduleId, string courtId)
    {
        if (courtId.Length == 0)
            return new ResultError("Empty Court Title");
        
        return new AddCourtToDailyScheduleCommand(existingScheduleId, courtId);
}

}