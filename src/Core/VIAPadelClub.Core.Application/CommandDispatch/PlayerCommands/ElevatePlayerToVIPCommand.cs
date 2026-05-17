using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.CommandDispatch.PlayerCommands;

public class ElevatePlayerToVIPCommand
{
    public Guid PlayerId { get; }
    public TimeInterval Duration { get; }
    private ElevatePlayerToVIPCommand(Guid playerId, TimeInterval duration)
    {
        PlayerId = playerId;
        Duration = duration;
    }

    public static Result<ElevatePlayerToVIPCommand> Create(Guid playerId, TimeInterval duration)
    {
        return new ElevatePlayerToVIPCommand(playerId,  duration);
    }
}