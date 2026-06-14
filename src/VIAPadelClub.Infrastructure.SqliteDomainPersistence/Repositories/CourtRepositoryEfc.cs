using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Domain.Aggregates.Courts;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Repositories;

namespace VIAPadelClub.Infrastructure.SqliteDomainPersistence.Repositories;

/// <summary>
/// Entity Framework Core implementation of the Court repository.
/// Extends the base repository class and implements Court-specific querying logic.
/// </summary>
public class CourtRepositoryEfc : RepositoryBase.RepositoryEfcBase<Court, CourtId>, ICourtRepository
{
    public CourtRepositoryEfc(DomainModelContext context) : base(context)
    {
    }

    /// <summary>
    /// Retrieves a Court by its identifier with all bookings loaded.
    /// Overrides the base method to include related bookings.
    /// </summary>
    /// <param name="courtId">The identifier of the court</param>
    /// <returns>The court aggregate with all bookings, or null if not found</returns>
    public override async Task<Court?> GetAsync(CourtId courtId)
    {
        return await Context.Set<Court>()
            .Include(c => c.Bookings)
            .FirstOrDefaultAsync(c => c.Id == courtId);
    }
}

