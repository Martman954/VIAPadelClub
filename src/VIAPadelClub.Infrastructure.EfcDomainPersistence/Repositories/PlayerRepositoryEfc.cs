using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Repositories;

namespace VIAPadelClub.Infrastructure.EfcDomainPersistence.Repositories;

public class PlayerRepositoryEfc(DomainModelContext context)
    : RepositoryBase.RepositoryEfcBase<Player, ViaEmail>(context), IPlayerRepository
{
    public override async Task<Player?> GetAsync(ViaEmail playerId)
    {
        return await Context.Set<Player>()
            .FirstOrDefaultAsync(p => p.Email.Value == playerId.Value);
    }
}



