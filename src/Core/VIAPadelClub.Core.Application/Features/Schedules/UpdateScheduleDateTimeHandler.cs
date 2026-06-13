using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;
using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Application.Features.Schedules;

internal class UpdateScheduleDateTimeHandler : ICommandHandler<UpdateScheduleDateTimeCommand>
{
    private readonly IScheduleRepo _scheduleRepo;

    public UpdateScheduleDateTimeHandler(IScheduleRepo scheduleRepo)
    {
        _scheduleRepo = scheduleRepo;
    }

    public async Task<Result> HandleAsync(UpdateScheduleDateTimeCommand dateTimeCommand)
    {
        var scheduleResult = await Result.Try(() => _scheduleRepo.GetSchedule(dateTimeCommand.ScheduleId));
        if (scheduleResult is Result<Schedule>.Failure)
            return Result.Failure("Schedule not found.", ErrorType.NotFound);
 
        var schedule = scheduleResult.Payload;

        var result = Result.Combine(
            schedule.UpdateDate(dateTimeCommand.ScheduleTimeInterval.TimeInterval.Start),
            schedule.UpdateTimes(dateTimeCommand.ScheduleTimeInterval.TimeInterval)
        );


        return result;
    }
}