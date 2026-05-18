using VIAPadelClub.Core.Domain.Aggregates.Schedules;

namespace VIAPadelClub.Core.Domain.Repositories;

public interface IScheduleRepo
{
    public Task<Schedule> AddSchedule(Schedule schedule);
    public Task<Schedule> GetSchedule(Guid scheduleId);
    public Task<Schedule> RemoveSchedule(Guid scheduleId);
}