using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;

public sealed class ActivateScheduleCommand
{
    public Guid ScheduleId { get; }

    private ActivateScheduleCommand(Guid scheduleId)
    {
        ScheduleId = scheduleId;
    }

    public static Result<ActivateScheduleCommand> Create(string scheduleId)
    {
        if (!Guid.TryParse(scheduleId, out var scheduleGuid))
            return Result.Failure<ActivateScheduleCommand>(new ResultError("Invalid schedule id format.", ErrorType.Validation));

        return Result.Success(new ActivateScheduleCommand(scheduleGuid));
    }
}

