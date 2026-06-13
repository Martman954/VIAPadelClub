using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;
using ScheduleAggregate = VIAPadelClub.Core.Domain.Aggregates.Schedules.Schedule;


namespace VIAPadelClub.Core.Application.Features.Schedules;

internal class AddCourtToScheduleHandler : ICommandHandler<AddCourtToScheduleCommand>
{
    private readonly IScheduleRepo _scheduleRepo;

    public AddCourtToScheduleHandler(IScheduleRepo scheduleRepo)
    {
        _scheduleRepo = scheduleRepo;
    }

    public async Task<Result> HandleAsync(AddCourtToScheduleCommand command)
    {
        var scheduleResult = await Result.Try(() => _scheduleRepo.GetSchedule(command.ScheduleId));
        if (scheduleResult is Result<ScheduleAggregate>.Failure)
            return Result.Failure("Schedule not found.", ErrorType.NotFound);
        
        var schedule = scheduleResult.Payload;
        var result = schedule.AddCourt(command.CourtId);


        return result;
    }
}

