using VIAPadelClub.Core.Application.CommandDispatch;
using VIAPadelClub.Core.Application.CommandDispatch.ScheduleCommands;
using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Application.Features.Schedules;

internal class UpdateScheduleDateTimeHandler : ICommandHandler<UpdateScheduleDateTimeCommand>
{
    private readonly IScheduleRepo _scheduleRepo;
    private readonly IUnitOfWork _unitOfWork;

    internal UpdateScheduleDateTimeHandler(IScheduleRepo scheduleRepo, IUnitOfWork unitOfWork)
    {
        _scheduleRepo = scheduleRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdateScheduleDateTimeCommand dateTimeCommand)
    {
        Result<Schedule> scheduleResult = await _scheduleRepo.GetSchedule(dateTimeCommand.ScheduleId);
        if (scheduleResult is Result<Schedule>.Failure f)
            return Result.Failure<None>(f.Errors);
        
        var schedule = scheduleResult.Payload;

        var result = Result.Combine(
            schedule.UpdateDate(dateTimeCommand.ScheduleTimeInterval.TimeInterval.Start),
            schedule.UpdateTimes(dateTimeCommand.ScheduleTimeInterval.TimeInterval)
        );

        if (result is Result<None>.Success)
            await _unitOfWork.SaveChangesAsync();

        return result;
    }
}