using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Repositories;

namespace VIAPadelClub.Infrastructure.SqliteDomainPersistence.Repositories;

/// <summary>
/// Entity Framework Core implementation of the Player repository.
/// Extends the base repository class and implements Player-specific querying logic.
/// </summary>
public class PlayerRepositoryEfc : RepositoryBase.RepositoryEfcBase<Player, ViaEmail>, IPlayerRepository
{
    public PlayerRepositoryEfc(DomainModelContext context) : base(context) { }

    /// <summary>
    /// Retrieves a Player by their unique identifier
    /// </summary>
    /// <param name="playerId">The unique identifier of the player</param>
    /// <returns>The player if found, or null if not found</returns>
    public override async Task<Player?> GetAsync(ViaEmail playerId)
    {
        return await Context.Set<Player>()
            .FirstOrDefaultAsync(p => p.Email.Value == playerId.Value);
    }
}



