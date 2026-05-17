using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.CommandDispatch.PlayerCommands;

public class BlacklistAPlayerCommand
{
    public Guid PlayerId { get; }
    private BlacklistAPlayerCommand(Guid playerId)
    {
        PlayerId = playerId;
    }

    public static Result<BlacklistAPlayerCommand> Create(Guid playerId)
    {
        return new BlacklistAPlayerCommand(playerId);
    }
}