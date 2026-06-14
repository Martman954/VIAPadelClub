using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Repositories;

/// <summary>
/// Repository interface for the Player aggregate root.
/// </summary>
public interface IPlayerRepository : IGenericRepository<Player, ViaEmail>
{
    /// <summary>
    /// Retrieves a Player by their unique identifier
    /// </summary>
    /// <param name="playerId">The unique identifier of the player</param>
    /// <returns>The player if found, or null if not found</returns>
    new Task<Player?> GetAsync(ViaEmail playerId);
}




