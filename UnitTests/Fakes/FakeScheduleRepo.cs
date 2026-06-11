using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Repositories;

namespace UnitTests.Fakes;

internal class FakeScheduleRepo : IScheduleRepo
{
    public List<Schedule> Schedules { get; } = [];

    public Task<Schedule> AddSchedule(Schedule schedule)
    {
        Schedules.Add(schedule);
        return Task.FromResult(schedule);
    }

    public Task<Schedule> GetSchedule(Guid scheduleId)
        => Task.FromResult(Schedules.First(s => s.Id == scheduleId));

    public Task<Schedule> RemoveSchedule(Guid scheduleId)
    {
        var schedule = Schedules.First(s => s.Id == scheduleId);
        Schedules.Remove(schedule);
        return Task.FromResult(schedule);
    }
}