using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Repositories;

namespace VIAPadelClub.Infrastructure.SqliteDomainPersistence.Repositories;

/// <summary>
/// Entity Framework Core implementation of the Schedule repository.
/// Extends the base repository class and implements Schedule-specific querying logic.
/// </summary>
public class ScheduleRepositoryEfc : RepositoryBase.RepositoryEfcBase<Schedule, ScheduleId>, IScheduleRepository
{
    public ScheduleRepositoryEfc(DomainModelContext context) : base(context)
    {
    }

    /// <summary>
    /// Retrieves a Schedule by its identifier with all time intervals and courts loaded.
    /// Overrides the base method to include related entities.
    /// </summary>
    /// <param name="scheduleId">The identifier of the schedule</param>
    /// <returns>The schedule aggregate with all dependent data, or null if not found</returns>
    public override async Task<Schedule?> GetAsync(ScheduleId scheduleId)
    {
        return await Context.Set<Schedule>()
            .FirstOrDefaultAsync(s => s.Id == scheduleId);
    }
}

