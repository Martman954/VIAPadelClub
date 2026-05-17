using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.CommandDispatch.PlayerCommands;

public class QuarantineAPlayerCommand
{
    public Guid PlayerId { get; }
    private QuarantineAPlayerCommand(Guid playerId)
    {
        PlayerId = playerId;
    }

    public static Result<QuarantineAPlayerCommand> Create(Guid playerId)
    {
        return new QuarantineAPlayerCommand(playerId);
    }
}