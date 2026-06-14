using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Application.AppEntry.CourtCommands;

public sealed class BookCourtCommand
{
    public ViaEmail PlayerId { get; }
    public CourtId CourtId { get; }
    public Guid ScheduleId { get; }
    public TimeInterval TimeInterval { get; }

    private BookCourtCommand(ViaEmail playerId, CourtId courtId, Guid scheduleId, TimeInterval timeInterval)
    {
        PlayerId = playerId;
        CourtId = courtId;
        ScheduleId = scheduleId;
        TimeInterval = timeInterval;
    }

    public static Result<BookCourtCommand> Create(string playerId, string courtId, string scheduleId, DateTime startTime, DateTime endTime)
    {
        var playerIdResult = ViaEmail.CreateEmail(playerId);

        var courtIdResult = CourtId.Create(courtId);

        var scheduleGuidResult = Guid.TryParse(scheduleId, out var scheduleGuid)
            ? Result.Success(scheduleGuid)
            : Result.Failure<Guid>(new ResultError("Invalid schedule id format.", ErrorType.Validation));

        var timeIntervalResult = TimeInterval.Create(startTime, endTime);

        return Result
            .CombineResultsInto<BookCourtCommand>(playerIdResult, courtIdResult, scheduleGuidResult, timeIntervalResult)
            .WithPayloadIfSuccess(() => new BookCourtCommand(
                playerIdResult.Payload,
                courtIdResult.Payload,
                scheduleGuidResult.Payload,
                timeIntervalResult.Payload));
    }
}

