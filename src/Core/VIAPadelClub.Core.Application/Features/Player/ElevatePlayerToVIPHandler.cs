using Features.CommandDispatch;
using Features.CommandDispatch.PlayerCommands;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;


public class ElevatePlayerToVIPHandler: ICommandHandler<ElevatePlayerToVIPCommand>
{
    
    private readonly IPlayerRepo _playerRepo;
    private readonly IUnitOfWork _unitOfWork;

    internal ElevatePlayerToVIPHandler(IPlayerRepo playerRepo, IUnitOfWork unitOfWork)
    {
        _playerRepo = playerRepo;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result> HandleAsync(ElevatePlayerToVIPCommand command)
    {
        // TODO: Implement
        
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
        
    }
}