using Features.CommandDispatch;
using Features.CommandDispatch.PlayerCommands;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;

internal class BlacklistAPlayerHandler :ICommandHandler<BlacklistAPlayerCommand>
{
    private readonly IPlayerRepo _playerRepo;
    private readonly IUnitOfWork _unitOfWork;

    internal BlacklistAPlayerHandler(IPlayerRepo playerRepo, IUnitOfWork unitOfWork)
    {
        _playerRepo = playerRepo;
        _unitOfWork = unitOfWork;
    }


    public async Task<Result> HandleAsync(BlacklistAPlayerCommand command)
    {
        // TODO: Implement 
        
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}