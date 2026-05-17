using Features.CommandDispatch;
using VIAPadelClub.Core.Domain.Aggregates.Schedule;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;


public class CancelABookingHandler : ICommandHandler<CancelABookingCommand>
{
    
    private readonly IScheduleRepo _scheduleRepo;
    private readonly IUnitOfWork _unitOfWork;

    internal CancelABookingHandler(IScheduleRepo scheduleRepo, IUnitOfWork unitOfWork)
    {
        _scheduleRepo = scheduleRepo;
        _unitOfWork = unitOfWork;
    }

    
    public async Task<Result> HandleAsync(CancelABookingCommand command)
    {
        
        Result<Schedule> existingSchedule = await _scheduleRepo.GetSchedule(command.ScheduleId);

        if (existingSchedule is Result<Schedule>.Failure f)
            return Result.Failure<None>(f.Errors);

        var schedule = existingSchedule.Payload;
        
        // TODO: remove bookings from schedule
        
        await _scheduleRepo.AddSchedule(schedule);
        await _scheduleRepo.RemoveSchedule(existingSchedule.Payload.Id);
        await _unitOfWork.SaveChangesAsync();
        
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}