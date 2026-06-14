using VIAPadelClub.Core.Domain.Aggregates.Courts;
using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Repositories;

public interface ICourtRepo
{
    /// <summary>
    /// Retrieves a fully-loaded Court aggregate with all its bookings.
    /// </summary>
    public Task<Court> GetCourt(CourtId courtId);
}

