using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;
using VIAPadelClub.Core.Application.ExternalServices;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.Services;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;
using CourtAggregate = VIAPadelClub.Core.Domain.Aggregates.Courts.Court;
using ScheduleAggregate = VIAPadelClub.Core.Domain.Aggregates.Schedules.Schedule;

namespace VIAPadelClub.Core.Application.Features.Schedules;

internal class RemoveCourtFromScheduleHandler(
    IScheduleRepository scheduleRepo,
    ICourtRepository courtRepo,
    ICourtRemovalNotifier courtRemovalNotifier)
    : ICommandHandler<RemoveCourtFromScheduleCommand>
{
    public async Task<Result> HandleAsync(RemoveCourtFromScheduleCommand command)
    {
        var scheduleResult = await Result.Try(() => scheduleRepo.GetAsync(ScheduleId.From(command.ScheduleId)));
        if (scheduleResult is Result<ScheduleAggregate>.Failure)
            return Result.Failure("Schedule not found.", ErrorType.NotFound);

        var courtResult = await Result.Try(() => courtRepo.GetAsync(command.CourtId));
        if (courtResult is Result<CourtAggregate>.Failure)
            return Result.Failure("Court not found.", ErrorType.NotFound);

        var schedule = scheduleResult.Payload;
        var court = courtResult.Payload;

        if (schedule == null || court == null)
            return Result.Failure("Schedule or Court not found.", ErrorType.NotFound);

        var removeResult = RemoveAvailableCourtFromSchedule.Handle(schedule, court, DateTime.Now);
        if (removeResult is Result<IReadOnlyList<ViaEmail>>.Failure f)
            return Result.Failure<None>(f.Errors);


        if (removeResult.Payload.Count == 0)
            return Result.Success();

        var notifyResult = await Result.Try(() =>
            courtRemovalNotifier.NotifyCourtRemovedAsync(removeResult.Payload, schedule.Id.GuidValue, court.Id));

        if (notifyResult is Result<None>.Failure)
            return Result.Failure("Court was removed, but notifying affected players failed.", ErrorType.Failure);

        return Result.Success();
    }
}



