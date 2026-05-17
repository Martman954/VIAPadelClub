using Features.CommandDispatch;
using Features.CommandDispatch.PlayerCommands;
using VIAPadelClub.Core.Domain.Contracts.Court;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.Features.Player;

internal class VIPMakeABookingHandler: ICommandHandler<VIPMakeABookingCommand>
{
    
    private readonly ICourtRepo _courtRepo;
    private readonly IUnitOfWork _unitOfWork;
    private ICourtHasBookingChecker _courtHasBookingChecker;
    

    internal VIPMakeABookingHandler(ICourtRepo courtRepo, IUnitOfWork unitOfWork, ICourtHasBookingChecker courtHasBookingChecker)
    {
        _courtRepo = courtRepo;
        _unitOfWork = unitOfWork;
        _courtHasBookingChecker = courtHasBookingChecker;
    }
    
    public async Task<Result> HandleAsync(VIPMakeABookingCommand command)
    {
        // TODO: Implement
        
        
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
        
    }
}