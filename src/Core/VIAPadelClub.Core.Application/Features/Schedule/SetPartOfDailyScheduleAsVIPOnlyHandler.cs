using Features.CommandDispatch;
using Features.CommandDispatch.ScheduleCommands;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.Features.Schedule;

internal class SetPartOfDailyScheduleAsVIPOnlyHandler: ICommandHandler<SetPartOfDailyScheduleAsVIPOnlyCommand>
{
    private readonly IScheduleRepo _scheduleRepo;
    private readonly IUnitOfWork _unitOfWork;

    internal SetPartOfDailyScheduleAsVIPOnlyHandler(IScheduleRepo scheduleRepo, IUnitOfWork unitOfWork)
    {
        _scheduleRepo = scheduleRepo;
        _unitOfWork = unitOfWork;
    }
    
    
    public async Task<Result> HandleAsync(SetPartOfDailyScheduleAsVIPOnlyCommand command)
    {
        // TODO: Implement
        
        
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}