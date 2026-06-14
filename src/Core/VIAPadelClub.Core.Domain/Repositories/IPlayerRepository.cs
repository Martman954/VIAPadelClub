using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Repositories;

public interface IPlayerRepository : IGenericRepository<Player, ViaEmail>
{
    new Task<Player?> GetAsync(ViaEmail playerId);
}




