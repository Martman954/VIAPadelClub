using Features.CommandDispatch;
using Features.CommandDispatch.ScheduleCommands;
using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

internal class UpdateScheduleHandler : ICommandHandler<UpdateScheduleCommand>
{
    //As a manager 
        //I want to update the time and date on a daily schedule.    
    
    private readonly IScheduleRepo _scheduleRepo;
    private readonly IUnitOfWork _unitOfWork;

    internal UpdateScheduleHandler(IScheduleRepo scheduleRepo, IUnitOfWork unitOfWork)
    {
        _scheduleRepo = scheduleRepo;
        _unitOfWork = unitOfWork;
    }
    

    public async Task<Result> HandleAsync(UpdateScheduleCommand command)
    {
        Result<Schedule> existingSchedule = await _scheduleRepo.GetSchedule(command.ScheduleId);

        var schedule = existingSchedule.Payload;
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}