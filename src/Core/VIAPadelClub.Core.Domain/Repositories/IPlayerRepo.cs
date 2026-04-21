using VIAPadelClub.Core.Domain.Aggregates.Player;

namespace VIAPadelClub.Core.Domain.Repositories;

public interface IPlayerRepo
{
    public Task<Player> AddPlayer(Player player);
    public Task<Player> GetPlayer(Guid playerId);
    public Task<Player> RemovePlayer(Guid playerId);
}