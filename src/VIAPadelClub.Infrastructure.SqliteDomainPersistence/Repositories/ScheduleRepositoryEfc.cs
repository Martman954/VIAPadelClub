using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Repositories;

namespace VIAPadelClub.Infrastructure.SqliteDomainPersistence.Repositories;

public class ScheduleRepositoryEfc(DomainModelContext context)
    : RepositoryBase.RepositoryEfcBase<Schedule, ScheduleId>(context), IScheduleRepository
{
    public override async Task<Schedule?> GetAsync(ScheduleId scheduleId)
    {
        return await Context.Set<Schedule>()
            .FirstOrDefaultAsync(s => s.Id == scheduleId);
    }
}

