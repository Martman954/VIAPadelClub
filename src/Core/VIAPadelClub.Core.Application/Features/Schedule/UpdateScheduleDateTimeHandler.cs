using Features.CommandDispatch;
using Features.CommandDispatch.ScheduleCommands;
using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;

internal class UpdateScheduleDateTimeHandler : ICommandHandler<UpdateScheduleDateTimeCommand>
{
    //As a manager 
        //I want to update the time and date on a daily schedule.    
    
    private readonly IScheduleRepo _scheduleRepo;
    private readonly IUnitOfWork _unitOfWork;

    internal UpdateScheduleDateTimeHandler(IScheduleRepo scheduleRepo, IUnitOfWork unitOfWork)
    {
        _scheduleRepo = scheduleRepo;
        _unitOfWork = unitOfWork;
    }
    

    public async Task<Result> HandleAsync(UpdateScheduleDateTimeCommand dateTimeCommand)
    {
        Result<Schedule> existingSchedule = await _scheduleRepo.GetSchedule(dateTimeCommand.ScheduleId);

        var schedule = existingSchedule.Payload;
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}