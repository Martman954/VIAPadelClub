using Features.CommandDispatch;
using Features.CommandDispatch.CourtCommands;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.Features.Schedule;

internal class RemoveAvailableCourtFromDailyScheduleHandler: ICommandHandler<RemoveAvailableCourtFromDailyScheduleCommand>
{
    private readonly IScheduleRepo _scheduleRepo;
    private readonly ICourtRepo _courtRepo;
    private readonly IUnitOfWork _unitOfWork;
    
    internal RemoveAvailableCourtFromDailyScheduleHandler(IScheduleRepo scheduleRepo, ICourtRepo courtRepo, IUnitOfWork unitOfWork)
    {
        _scheduleRepo = scheduleRepo;
        _courtRepo = courtRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(RemoveAvailableCourtFromDailyScheduleCommand command)
    {
        // TODO: Setup a command
        
        
        
        await _unitOfWork.SaveChangesAsync();
        return Result.Success(); 
    }
}