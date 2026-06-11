using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Application.CommandDispatch.ScheduleCommands;

public sealed class AddCourtToScheduleCommand
{
    public Guid ScheduleId { get; }
    public CourtId CourtId { get; }

    private AddCourtToScheduleCommand(Guid scheduleId, CourtId courtId)
    {
        ScheduleId = scheduleId;
        CourtId = courtId;
    }

    public static Result<AddCourtToScheduleCommand> Create(string scheduleId, string courtId)
    {
        var scheduleGuidResult = Guid.TryParse(scheduleId, out var scheduleGuid)
            ? Result.Success(scheduleGuid)
            : Result.Failure<Guid>(new ResultError("Invalid schedule id format.", ErrorType.Validation));

        var courtIdResult = CourtId.Create(courtId);

        return Result
            .CombineResultsInto<AddCourtToScheduleCommand>(scheduleGuidResult, courtIdResult)
            .WithPayloadIfSuccess(() => new AddCourtToScheduleCommand(scheduleGuidResult.Payload, courtIdResult.Payload));
    }
}

