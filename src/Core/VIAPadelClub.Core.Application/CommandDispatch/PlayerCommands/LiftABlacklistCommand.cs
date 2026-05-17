using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.CommandDispatch.PlayerCommands;

public class LiftABlacklistCommand
{
    public Guid PlayerId { get; }
    private LiftABlacklistCommand(Guid playerId)
    {
        PlayerId = playerId;
    }

    public static Result<LiftABlacklistCommand> Create(Guid playerId)
    {
        return new LiftABlacklistCommand(playerId);
    }
}