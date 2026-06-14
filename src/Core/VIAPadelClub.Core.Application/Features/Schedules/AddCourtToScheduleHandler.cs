using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;
using ScheduleAggregate = VIAPadelClub.Core.Domain.Aggregates.Schedules.Schedule;


namespace VIAPadelClub.Core.Application.Features.Schedules;

internal class AddCourtToScheduleHandler(IScheduleRepository scheduleRepo) : ICommandHandler<AddCourtToScheduleCommand>
{
    public async Task<Result> HandleAsync(AddCourtToScheduleCommand command)
    {
        var scheduleResult = await Result.Try(() => scheduleRepo.GetAsync(ScheduleId.From(command.ScheduleId)));
        if (scheduleResult is Result<ScheduleAggregate>.Failure)
            return Result.Failure("Schedule not found.", ErrorType.NotFound);
        
        var schedule = scheduleResult.Payload;
        if (schedule == null)
            return Result.Failure("Schedule not found.", ErrorType.NotFound);
        var result = schedule.AddCourt(command.CourtId);


        return result;
    }
}

