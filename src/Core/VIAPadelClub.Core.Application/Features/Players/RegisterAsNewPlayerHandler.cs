using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.PlayerCommands;
using PlayerAggregate = VIAPadelClub.Core.Domain.Aggregates.Players.Player;
using VIAPadelClub.Core.Domain.Contracts.Players;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Application.Features.Players;

public class RegisterAsNewPlayerHandler(
    IPlayerRepo playerRepository,
    IEmailInUseChecker emailInUseChecker)
    : ICommandHandler<RegisterAsNewPlayerCommand>
{
    public async Task<Result> HandleAsync(RegisterAsNewPlayerCommand command)
    {
        var playerResult = PlayerAggregate.Register(command.Email, command.Name, command.ImageUrl, emailInUseChecker);
        if (playerResult is Result<PlayerAggregate>.Failure f)
            return Result.Failure<None>(f.Errors);

        await playerRepository.AddPlayer(playerResult.Payload);

        return Result.Success();
    }
}