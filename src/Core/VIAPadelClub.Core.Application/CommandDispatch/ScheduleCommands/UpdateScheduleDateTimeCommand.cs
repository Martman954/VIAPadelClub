using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using ScheduleTimeInterval = VIAPadelClub.Core.Domain.Aggregates.Schedules.ValueObjects.ScheduleTimeInterval;


namespace VIAPadelClub.Core.Application.CommandDispatch.ScheduleCommands;

public class UpdateScheduleDateTimeCommand
{
    public Guid ScheduleId { get;}
    public ScheduleTimeInterval ScheduleTimeInterval { get; }
    
    private UpdateScheduleDateTimeCommand(Guid scheduleId ,ScheduleTimeInterval scheduleTimeInterval)
    {
        ScheduleId = scheduleId;
        ScheduleTimeInterval = scheduleTimeInterval;
    }

    public static Result<UpdateScheduleDateTimeCommand> Create(string scheduleId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        var scheduleGuidResult = Guid.TryParse(scheduleId, out var scheduleGuid)
            ? Result.Success(scheduleGuid)
            : Result.Failure<Guid>(new ResultError("Invalid scheduleId format."));

        var timeIntervalResult = TimeInterval.Create(new DateTime(date, startTime), new DateTime(date, endTime));
        if (timeIntervalResult is Result<TimeInterval>.Failure f1)
            return Result.Failure<UpdateScheduleDateTimeCommand>(f1.Errors);

        var scheduleTimeIntervalResult = ScheduleTimeInterval.Create(timeIntervalResult.Payload);

        return Result
            .CombineResultsInto<UpdateScheduleDateTimeCommand>(scheduleGuidResult, scheduleTimeIntervalResult)
            .WithPayloadIfSuccess(() => new UpdateScheduleDateTimeCommand(scheduleGuidResult.Payload, scheduleTimeIntervalResult.Payload));
    }
}