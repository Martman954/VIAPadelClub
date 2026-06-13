using VIAPadelClub.Core.Application.CommandDispatch;
using VIAPadelClub.Core.Application.CommandDispatch.ScheduleCommands;
using VIAPadelClub.Core.Application.ExternalServices;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.Services;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;
using CourtAggregate = VIAPadelClub.Core.Domain.Aggregates.Courts.Court;
using ScheduleAggregate = VIAPadelClub.Core.Domain.Aggregates.Schedules.Schedule;

namespace VIAPadelClub.Core.Application.Features.Schedules;

internal class RemoveCourtFromScheduleHandler : ICommandHandler<RemoveCourtFromScheduleCommand>
{
    private readonly IScheduleRepo _scheduleRepo;
    private readonly ICourtRepo _courtRepo;
    private readonly ICourtRemovalNotifier _courtRemovalNotifier;
    private readonly IUnitOfWork _unitOfWork;

    internal RemoveCourtFromScheduleHandler(
        IScheduleRepo scheduleRepo,
        ICourtRepo courtRepo,
        ICourtRemovalNotifier courtRemovalNotifier,
        IUnitOfWork unitOfWork)
    {
        _scheduleRepo = scheduleRepo;
        _courtRepo = courtRepo;
        _courtRemovalNotifier = courtRemovalNotifier;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(RemoveCourtFromScheduleCommand command)
    {
        var scheduleResult = await Result.Try(() => _scheduleRepo.GetSchedule(command.ScheduleId));
        if (scheduleResult is Result<ScheduleAggregate>.Failure)
            return Result.Failure("Schedule not found.", ErrorType.NotFound);

        var courtResult = await Result.Try(() => _courtRepo.GetCourt(command.CourtId));
        if (courtResult is Result<CourtAggregate>.Failure)
            return Result.Failure("Court not found.", ErrorType.NotFound);

        var schedule = scheduleResult.Payload;
        var court = courtResult.Payload;

        var removeResult = RemoveAvailableCourtFromSchedule.Handle(schedule, court, DateTime.Now);
        if (removeResult is Result<IReadOnlyList<ViaEmail>>.Failure f)
            return Result.Failure<None>(f.Errors);

        await _unitOfWork.SaveChangesAsync();

        if (removeResult.Payload.Count == 0)
            return Result.Success();

        var notifyResult = await Result.Try(() =>
            _courtRemovalNotifier.NotifyCourtRemovedAsync(removeResult.Payload, schedule.Id, court.Id));

        if (notifyResult is Result<None>.Failure)
            return Result.Failure("Court was removed, but notifying affected players failed.", ErrorType.Failure);

        return Result.Success();
    }
}



