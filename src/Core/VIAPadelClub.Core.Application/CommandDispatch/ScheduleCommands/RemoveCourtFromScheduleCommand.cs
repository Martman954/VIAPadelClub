using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Application.CommandDispatch.ScheduleCommands;

public sealed class RemoveCourtFromScheduleCommand
{
    public Guid ScheduleId { get; }
    public CourtId CourtId { get; }

    private RemoveCourtFromScheduleCommand(Guid scheduleId, CourtId courtId)
    {
        ScheduleId = scheduleId;
        CourtId = courtId;
    }

    public static Result<RemoveCourtFromScheduleCommand> Create(string scheduleId, string courtId)
    {
        var scheduleGuidResult = Guid.TryParse(scheduleId, out var scheduleGuid)
            ? Result.Success(scheduleGuid)
            : Result.Failure<Guid>(new ResultError("Invalid schedule id format.", ErrorType.Validation));

        var courtIdResult = CourtId.Create(courtId);

        return Result
            .CombineResultsInto<RemoveCourtFromScheduleCommand>(scheduleGuidResult, courtIdResult)
            .WithPayloadIfSuccess(() => new RemoveCourtFromScheduleCommand(scheduleGuidResult.Payload, courtIdResult.Payload));
    }
}

