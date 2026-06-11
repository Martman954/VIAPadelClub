using VIAPadelClub.Core.Application.CommandDispatch;
using VIAPadelClub.Core.Application.CommandDispatch.ScheduleCommands;
using VIAPadelClub.Core.Domain.Contracts.Schedules;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;
using ScheduleAggregate = VIAPadelClub.Core.Domain.Aggregates.Schedules.Schedule;

namespace VIAPadelClub.Core.Application.Features.Schedules;

internal class ActivateScheduleHandler : ICommandHandler<ActivateScheduleCommand>
{
    private readonly IScheduleRepo _scheduleRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IScheduleDateConflictChecker _dateConflictChecker;

    internal ActivateScheduleHandler(
        IScheduleRepo scheduleRepo,
        IUnitOfWork unitOfWork,
        IScheduleDateConflictChecker dateConflictChecker)
    {
        _scheduleRepo = scheduleRepo;
        _unitOfWork = unitOfWork;
        _dateConflictChecker = dateConflictChecker;
    }

    public async Task<Result> HandleAsync(ActivateScheduleCommand command)
    {
        var scheduleResult = await Result.Try(() => _scheduleRepo.GetSchedule(command.ScheduleId));
        if (scheduleResult is Result<ScheduleAggregate>.Failure)
            return Result.Failure("Schedule not found.", ErrorType.NotFound);

        var schedule = scheduleResult.Payload;
        var result = schedule.Activate(_dateConflictChecker);

        if (result is Result<None>.Success)
            await _unitOfWork.SaveChangesAsync();

        return result;
    }
}

