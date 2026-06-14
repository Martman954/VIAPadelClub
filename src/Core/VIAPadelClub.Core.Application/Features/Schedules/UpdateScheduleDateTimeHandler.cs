using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;
using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Application.Features.Schedules;

internal class UpdateScheduleDateTimeHandler(IScheduleRepository scheduleRepo)
    : ICommandHandler<UpdateScheduleDateTimeCommand>
{
    public async Task<Result> HandleAsync(UpdateScheduleDateTimeCommand dateTimeCommand)
    {
        var scheduleResult = await Result.Try(() => scheduleRepo.GetAsync(ScheduleId.From(dateTimeCommand.ScheduleId)));
        if (scheduleResult is Result<Schedule>.Failure)
            return Result.Failure("Schedule not found.", ErrorType.NotFound);
 
        var schedule = scheduleResult.Payload;
        if (schedule == null)
            return Result.Failure("Schedule not found.", ErrorType.NotFound);

        var result = Result.Combine(
            schedule.UpdateDate(dateTimeCommand.ScheduleTimeInterval.TimeInterval.Start),
            schedule.UpdateTimes(dateTimeCommand.ScheduleTimeInterval.TimeInterval)
        );


        return result;
    }
}