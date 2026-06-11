using Features.CommandDispatch;
using Features.CommandDispatch.ScheduleCommands;
using ScheduleAggregate = VIAPadelClub.Core.Domain.Aggregates.Schedules.Schedule;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.Features.Schedule;

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
        Result<ScheduleAggregate> newSchedule = ScheduleAggregate.Create();
        
        if (newSchedule is Result<ScheduleAggregate>.Failure f)
            return Result.Failure<None>(f.Errors);
        
        await _scheduleRepo.AddSchedule(newSchedule.Payload);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success(); 
    }
}