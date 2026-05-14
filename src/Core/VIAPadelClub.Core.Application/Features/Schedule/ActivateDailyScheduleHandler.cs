using Features.CommandDispatch;
using Features.CommandDispatch.ScheduleCommands;
using VIAPadelClub.Core.Domain.Aggregates.Schedule;
using VIAPadelClub.Core.Domain.Contracts.Schedule;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;


internal class ActivateDailyScheduleHandler: ICommandHandler<ActivateDailyScheduleCommand>
{
    
    private readonly IScheduleRepo _scheduleRepo;
    private readonly IUnitOfWork _unitOfWork;
    
    private IScheduleDateConflictChecker _conflictChecker;
    internal ActivateDailyScheduleHandler(IScheduleRepo scheduleRepo, IUnitOfWork unitOfWork,IScheduleDateConflictChecker conflictChecker)
    {
        _scheduleRepo = scheduleRepo;
        _unitOfWork = unitOfWork;
        _conflictChecker = conflictChecker;
    }
    public async Task<Result> HandleAsync(ActivateDailyScheduleCommand command)
    {

        Result<Schedule> existingSchedule = await _scheduleRepo.GetSchedule(command.ScheduleId);

        var schedule = existingSchedule.Payload;
        schedule.Activate(_conflictChecker);
        
        await _scheduleRepo.RemoveSchedule(existingSchedule.Payload.Id);
        await _scheduleRepo.AddSchedule(schedule);
        
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}