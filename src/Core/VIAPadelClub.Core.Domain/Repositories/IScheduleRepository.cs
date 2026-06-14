using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Repositories;

/// <summary>
/// Repository interface for the Schedule aggregate root.
/// Inherits from the generic repository interface with Schedule-specific constraints.
/// </summary>
public interface IScheduleRepository : IGenericRepository<Schedule, ScheduleId>
{
    /// <summary>
    /// Retrieves a Schedule by its identifier with all time intervals and courts loaded.
    /// </summary>
    /// <param name="scheduleId">The identifier of the schedule</param>
    /// <returns>The schedule aggregate with all dependent data, or null if not found</returns>
    new Task<Schedule?> GetAsync(ScheduleId scheduleId);
}

