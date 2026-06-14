using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;
using ScheduleAggregate = VIAPadelClub.Core.Domain.Aggregates.Schedules.Schedule;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Application.Features.Schedules;

internal class CreateScheduleHandler(IScheduleRepository scheduleRepo) : ICommandHandler<CreateScheduleCommand>
{
    public async Task<Result> HandleAsync(CreateScheduleCommand command)
    {
        var newSchedule = ScheduleAggregate.Create();
        
        if (newSchedule is Result<ScheduleAggregate>.Failure f)
            return Result.Failure<None>(f.Errors);
        
        return await Result.Try(async () =>
        {
            await scheduleRepo.AddAsync(newSchedule.Payload);
        });
    }
}