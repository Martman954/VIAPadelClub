using Features.CommandDispatch;
using Features.CommandDispatch.PlayerCommands;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.Features.Player;

internal class PlayerMakesABookingHandler: ICommandHandler<PlayerMakesABookingCommand>
{
    private readonly ICourtRepo _courtRepo;
    private readonly IUnitOfWork _unitOfWork;

    internal PlayerMakesABookingHandler(ICourtRepo courtRepo, IUnitOfWork unitOfWork)
    {
        _courtRepo = courtRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(PlayerMakesABookingCommand command)
    {
        
        
        
        
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}