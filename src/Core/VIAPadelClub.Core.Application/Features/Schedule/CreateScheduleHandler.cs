using Features.CommandDispatch;
using Features.CommandDispatch.ScheduleCommands;
using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;


internal class CreateScheduleHandler : ICommandHandler<CreateScheduleCommand>
{
    // The manager creates a new daily schedule
        //And the status is set to “draft” 
        //And the list of available courts is empty 
        //And the times are set to 15:00 and 22:00  
        //And date is set to today 
    private readonly IScheduleRepo _scheduleRepo;
    private readonly IUnitOfWork _unitOfWork;

    internal CreateScheduleHandler(IScheduleRepo scheduleRepo, IUnitOfWork unitOfWork)
    {
        _scheduleRepo = scheduleRepo;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result> HandleAsync(CreateScheduleCommand command)
    {
        Result<Schedule> newSchedule = Schedule.Create();
        
        if (newSchedule is Result<Schedule>.Failure f)
            return Result.Failure<None>(f.Errors);
        
        await _scheduleRepo.AddSchedule(newSchedule.Payload);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success(); 
    }
}