using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Repositories;

public interface IScheduleRepository : IGenericRepository<Schedule, ScheduleId>
{
    new Task<Schedule?> GetAsync(ScheduleId scheduleId);
}

