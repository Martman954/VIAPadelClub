using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.QueryContracts;
using VIAPadelClub.Infrastructure.EfcDomainPersistence;

namespace VIAPadelClub.Infrastructure.EfcQueries;

public sealed class ManagerScheduleOverviewQueryHandler(DomainModelContext context)
    : IQueryHandler<ManagerScheduleOverview.Query, ManagerScheduleOverview.Answer>
{
    public async Task<ManagerScheduleOverview.Answer> HandleAsync(ManagerScheduleOverview.Query query)
    {
        var schedules = await context.Set<Schedule>().ToListAsync();

        var first = new DateOnly(query.Year, query.Month, 1);
        var daysInMonth = DateTime.DaysInMonth(query.Year, query.Month);

        var statusByDate = schedules
            .Where(s => s.Times.Count > 0)
            .GroupBy(s => DateOnly.FromDateTime(s.Times.Min(t => t.TimeInterval.Start)))
            .ToDictionary(g => g.Key, g => g.First().Status.ToString());

        var dayStatuses = Enumerable.Range(0, daysInMonth)
            .Select(offset => first.AddDays(offset))
            .Select(date => new ManagerScheduleOverview.DayStatusItem(
                date,
                statusByDate.GetValueOrDefault(date)))
            .ToList();

        return new ManagerScheduleOverview.Answer(query.Year, query.Month, dayStatuses);
    }
}