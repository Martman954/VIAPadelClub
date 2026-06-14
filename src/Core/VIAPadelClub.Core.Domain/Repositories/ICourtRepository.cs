using VIAPadelClub.Core.Domain.Aggregates.Courts;
using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Repositories;

/// <summary>
/// Repository interface for the Court aggregate root.
/// Inherits from the generic repository interface with Court-specific constraints.
/// </summary>
public interface ICourtRepository : IGenericRepository<Court, CourtId>
{
    /// <summary>
    /// Retrieves a Court by its identifier with all bookings loaded.
    /// </summary>
    /// <param name="courtId">The identifier of the court</param>
    /// <returns>The court aggregate with bookings, or null if not found</returns>
    new Task<Court?> GetAsync(CourtId courtId);
}

