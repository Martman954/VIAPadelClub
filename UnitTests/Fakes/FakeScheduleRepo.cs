using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Repositories;

namespace UnitTests.Fakes;

internal class FakeScheduleRepo : IScheduleRepository
{
    public List<Schedule> Schedules { get; } = [];

    public Task AddAsync(Schedule schedule)
    {
        Schedules.Add(schedule);
        return Task.CompletedTask;
    }

    public Task<Schedule?> GetAsync(ScheduleId scheduleId)
        => Task.FromResult(Schedules.FirstOrDefault(s => s.Id.Equals(scheduleId)));

    public Task RemoveAsync(ScheduleId scheduleId)
    {
        var schedule = Schedules.FirstOrDefault(s => s.Id.Equals(scheduleId));
        if (schedule != null)
            Schedules.Remove(schedule);
        return Task.CompletedTask;
    }
}