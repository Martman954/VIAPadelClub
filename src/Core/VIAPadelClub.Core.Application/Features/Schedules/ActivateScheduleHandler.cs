using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;
using VIAPadelClub.Core.Domain.Contracts.Schedules;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;
using ScheduleAggregate = VIAPadelClub.Core.Domain.Aggregates.Schedules.Schedule;

namespace VIAPadelClub.Core.Application.Features.Schedules;

internal class ActivateScheduleHandler : ICommandHandler<ActivateScheduleCommand>
{
    private readonly IScheduleRepo _scheduleRepo;
    private readonly IScheduleDateConflictChecker _dateConflictChecker;

    public ActivateScheduleHandler(
        IScheduleRepo scheduleRepo,
        IScheduleDateConflictChecker dateConflictChecker)
    {
        _scheduleRepo = scheduleRepo;
        _dateConflictChecker = dateConflictChecker;
    }

    // TODO: make stuff internal here
    public async Task<Result> HandleAsync(ActivateScheduleCommand command)
    {
        var scheduleResult = await Result.Try(() => _scheduleRepo.GetSchedule(command.ScheduleId));
        if (scheduleResult is Result<ScheduleAggregate>.Failure)
            return Result.Failure("Schedule not found.", ErrorType.NotFound);

        var schedule = scheduleResult.Payload;
        var result = schedule.Activate(_dateConflictChecker);


        return result;
    }
}

