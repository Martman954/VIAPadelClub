using Features.CommandDispatch;
using Features.CommandDispatch.PlayerCommands;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.Features.Player;

public class LiftABlacklistHandler:ICommandHandler<LiftABlacklistCommand>
{
    private readonly IPlayerRepo _playerRepo;
    private readonly IUnitOfWork _unitOfWork;

    internal LiftABlacklistHandler(IPlayerRepo playerRepo, IUnitOfWork unitOfWork)
    {
        _playerRepo = playerRepo;
        _unitOfWork = unitOfWork;
    }

    
    public async Task<Result> HandleAsync(LiftABlacklistCommand command)
    {
        // TODO: Implement
        
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}