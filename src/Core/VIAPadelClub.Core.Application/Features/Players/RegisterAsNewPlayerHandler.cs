using VIAPadelClub.Core.Application.CommandDispatch;
using VIAPadelClub.Core.Application.CommandDispatch.PlayerCommands;
using PlayerAggregate = VIAPadelClub.Core.Domain.Aggregates.Players.Player;
using VIAPadelClub.Core.Domain.Contracts.Players;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Application.Features.Players;

public class RegisterAsNewPlayerHandler(
    IPlayerRepo playerRepository,
    IUnitOfWork unitOfWork,
    IEmailInUseChecker emailInUseChecker)
    : ICommandHandler<RegisterAsNewPlayerCommand>
{
    public async Task<Result> HandleAsync(RegisterAsNewPlayerCommand command)
    {
        // 1. Create the domain object via your value objects
        var playerResult = PlayerAggregate.Register(command.Email, command.Name, command.ImageUrl, emailInUseChecker);

        // 2. Add to repository (in-memory, no DB call yet)
        await playerRepository.AddPlayer(playerResult.Payload);

        // 3. Commit the transaction (actual DB call happens here)
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}