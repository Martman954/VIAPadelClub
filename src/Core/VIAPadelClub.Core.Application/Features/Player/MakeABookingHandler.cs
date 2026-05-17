using Features.CommandDispatch;
using Features.CommandDispatch.PlayerCommands;
using VIAPadelClub.Core.Domain.Aggregates.Court;
using VIAPadelClub.Core.Domain.Aggregates.Court.Entities;
using VIAPadelClub.Core.Domain.Contracts.Court;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.Features.Player;

internal class MakeABookingHandler: ICommandHandler<MakeABookingCommand>
{
    private readonly ICourtRepo _courtRepo;
    private readonly IUnitOfWork _unitOfWork;
    private ICourtHasBookingChecker _courtHasBookingChecker;
    

    internal MakeABookingHandler(ICourtRepo courtRepo, IUnitOfWork unitOfWork, ICourtHasBookingChecker courtHasBookingChecker)
    {
        _courtRepo = courtRepo;
        _unitOfWork = unitOfWork;
        _courtHasBookingChecker = courtHasBookingChecker;
    }

    public async Task<Result> HandleAsync(MakeABookingCommand command)
    {
        // TODO: Finish booking shit 
        
        
        
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}