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
    
    private IScheduleDateConflictChecker confluentChecker;
    internal ActivateDailyScheduleHandler(IScheduleRepo scheduleRepo, IUnitOfWork unitOfWork,IScheduleDateConflictChecker confluentChecker)
    {
        _scheduleRepo = scheduleRepo;
        _unitOfWork = unitOfWork;
        this.confluentChecker = confluentChecker;
    }
    public async Task<Result> HandleAsync(ActivateDailyScheduleCommand command)
    {

        Result<Schedule> existingSchedule = await _scheduleRepo.GetSchedule(command.ScheduleId);

        
        
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}