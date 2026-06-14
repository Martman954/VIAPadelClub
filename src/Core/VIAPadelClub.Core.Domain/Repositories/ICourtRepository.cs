using VIAPadelClub.Core.Domain.Aggregates.Courts;
using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Repositories;

public interface ICourtRepository : IGenericRepository<Court, CourtId>
{
    new Task<Court?> GetAsync(CourtId courtId);
}

