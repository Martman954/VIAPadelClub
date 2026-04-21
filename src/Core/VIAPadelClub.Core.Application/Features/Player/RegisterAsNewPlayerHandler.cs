using Features.CommandDispatch;
using Features.CommandDispatch.PlayerCommands;
using VIAPadelClub.Core.Domain.Contracts;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.Features.Player;

public class RegisterAsNewPlayerHandler : ICommandHandler<RegisterAsNewPlayerCommand>
{
    private readonly IPlayerRepo _playerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailInUseChecker _emailInUseChecker;
    
    public RegisterAsNewPlayerHandler(IPlayerRepo playerRepository, IUnitOfWork unitOfWork,  IEmailInUseChecker emailInUseChecker)
    {
        _playerRepository = playerRepository;
        _unitOfWork = unitOfWork;
        _emailInUseChecker = emailInUseChecker;
    }

    public async Task<Result> HandleAsync(RegisterAsNewPlayerCommand command)
    {
        // 1. Create the domain object via your value objects
        var playerResult = VIAPadelClub.Core.Domain.Aggregates.Player.Player.Register(command.Email, command.Name, command.ImageUrl, _emailInUseChecker);

        // 2. Add to repository (in-memory, no DB call yet)
        await _playerRepository.AddPlayer(playerResult.Payload);

        // 3. Commit the transaction (actual DB call happens here)
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}