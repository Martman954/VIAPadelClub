using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Domain.Aggregates.Courts;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Repositories;

namespace VIAPadelClub.Infrastructure.EfcDomainPersistence.Repositories;

public class CourtRepositoryEfc(DomainModelContext context)
    : RepositoryBase.RepositoryEfcBase<Court, CourtId>(context), ICourtRepository
{
    public override async Task<Court?> GetAsync(CourtId courtId)
    {
        return await Context.Set<Court>()
            .Include(c => c.Bookings)
            .FirstOrDefaultAsync(c => c.Id == courtId);
    }
}

