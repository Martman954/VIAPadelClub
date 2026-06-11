using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Application.CommandDispatch.CourtCommands;

public sealed class BookCourtCommand
{
    public Guid PlayerId { get; }
    public CourtId CourtId { get; }
    public Guid ScheduleId { get; }
    public TimeInterval TimeInterval { get; }

    private BookCourtCommand(Guid playerId, CourtId courtId, Guid scheduleId, TimeInterval timeInterval)
    {
        PlayerId = playerId;
        CourtId = courtId;
        ScheduleId = scheduleId;
        TimeInterval = timeInterval;
    }

    public static Result<BookCourtCommand> Create(string playerId, string courtId, string scheduleId, DateTime startTime, DateTime endTime)
    {
        var playerGuidResult = Guid.TryParse(playerId, out var playerGuid)
            ? Result.Success(playerGuid)
            : Result.Failure<Guid>(new ResultError("Invalid player id format.", ErrorType.Validation));

        var courtIdResult = CourtId.Create(courtId);

        var scheduleGuidResult = Guid.TryParse(scheduleId, out var scheduleGuid)
            ? Result.Success(scheduleGuid)
            : Result.Failure<Guid>(new ResultError("Invalid schedule id format.", ErrorType.Validation));

        var timeIntervalResult = TimeInterval.Create(startTime, endTime);

        return Result
            .CombineResultsInto<BookCourtCommand>(playerGuidResult, courtIdResult, scheduleGuidResult, timeIntervalResult)
            .WithPayloadIfSuccess(() => new BookCourtCommand(
                playerGuidResult.Payload,
                courtIdResult.Payload,
                scheduleGuidResult.Payload,
                timeIntervalResult.Payload));
    }
}

