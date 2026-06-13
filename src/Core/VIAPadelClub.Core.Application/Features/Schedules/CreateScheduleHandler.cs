using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;
using ScheduleAggregate = VIAPadelClub.Core.Domain.Aggregates.Schedules.Schedule;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Application.Features.Schedules;

internal class CreateScheduleHandler : ICommandHandler<CreateScheduleCommand>
{
    private readonly IScheduleRepo _scheduleRepo;
    private readonly IUnitOfWork _unitOfWork;

    internal CreateScheduleHandler(IScheduleRepo scheduleRepo, IUnitOfWork unitOfWork)
    {
        _scheduleRepo = scheduleRepo;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result> HandleAsync(CreateScheduleCommand command)
    {
        var newSchedule = ScheduleAggregate.Create();
        
        if (newSchedule is Result<ScheduleAggregate>.Failure f)
            return Result.Failure<None>(f.Errors);
        
        return await Result.Try(async () =>
        {
            await _scheduleRepo.AddSchedule(newSchedule.Payload);
            await _unitOfWork.SaveChangesAsync();
        });
    }
}