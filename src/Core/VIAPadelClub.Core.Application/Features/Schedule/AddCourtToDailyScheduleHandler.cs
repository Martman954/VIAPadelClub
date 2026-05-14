using Features.CommandDispatch;
using Features.CommandDispatch.ScheduleCommands;
using VIAPadelClub.Core.Domain.Aggregates.Court;
using VIAPadelClub.Core.Domain.Aggregates.Schedule;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;


internal class AddCourtToDailyScheduleHandler: ICommandHandler<AddCourtToDailyScheduleCommand>
{
    private readonly IScheduleRepo _scheduleRepo;
    private readonly ICourtRepo _courtRepo;
    private readonly IUnitOfWork _unitOfWork;
    
    internal AddCourtToDailyScheduleHandler(IScheduleRepo scheduleRepo, ICourtRepo courtRepo, IUnitOfWork unitOfWork)
    {
        _scheduleRepo = scheduleRepo;
        _courtRepo = courtRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(AddCourtToDailyScheduleCommand command)
    {
        Result<Schedule> existingDailySchedule = await _scheduleRepo.GetSchedule(command.ExistingScheduleId);
        Result<CourtId> newCourt = CourtId.Create(command.CourtId);

        if (existingDailySchedule is Result<Schedule>.Failure f)
            return Result.Failure<None>(f.Errors);
        if(newCourt is Result<CourtId>.Failure f2)
            return Result.Failure<None>(f2.Errors);

        var schedule = existingDailySchedule.Payload;
        
        var updateResult = schedule.AddCourt(newCourt.Payload);
        if (updateResult is Result<None>.Failure f3)
            return Result.Failure<None>(f3.Errors);
        
        await _scheduleRepo.AddSchedule(schedule);
        await _scheduleRepo.RemoveSchedule(existingDailySchedule.Payload.Id);
        
        await _courtRepo.AddCourt(Court.Create(command.CourtId).Payload);
        
        await _unitOfWork.SaveChangesAsync();
        return Result.Success(); 
    }
}