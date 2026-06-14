using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Domain.Aggregates.Courts;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.QueryContracts;
using VIAPadelClub.Infrastructure.EfcDomainPersistence;

namespace VIAPadelClub.Infrastructure.EfcQueries;

public sealed class ScheduleBookingOverviewQueryHandler(DomainModelContext context)
    : IQueryHandler<ScheduleBookingOverview.Query, ScheduleBookingOverview.Answer>
{
    public async Task<ScheduleBookingOverview.Answer> HandleAsync(ScheduleBookingOverview.Query query)
    {
        var schedules = await context.Set<Schedule>().ToListAsync();
        var courts = await context.Set<Court>().ToListAsync();
        var players = await context.Set<Player>().ToListAsync();

        var schedule = schedules.FirstOrDefault(s =>
            s.Times.Count > 0 &&
            DateOnly.FromDateTime(s.Times.Min(t => t.TimeInterval.Start)) == query.Date);

        if (schedule == null)
            return new ScheduleBookingOverview.Answer(query.Date, []);

        var scheduleStart = schedule.Times.Min(t => t.TimeInterval.Start);
        var scheduleEnd = schedule.Times.Max(t => t.TimeInterval.End);
        var vipIntervals = schedule.VipTimes.Select(v => v.TimeInterval).ToList();

        var columns = schedule.Courts
            .Select(courtId => courts.FirstOrDefault(c => c.Id.Equals(courtId)))
            .Where(c => c != null)
            .Select(court => BuildCourtColumn(court!, schedule, scheduleStart, scheduleEnd, vipIntervals, players))
            .ToList();

        return new ScheduleBookingOverview.Answer(query.Date, columns);
    }

    private static ScheduleBookingOverview.CourtColumn BuildCourtColumn(
        Court court,
        Schedule schedule,
        DateTime scheduleStart,
        DateTime scheduleEnd,
        IReadOnlyList<TimeInterval> vipIntervals,
        IReadOnlyList<Player> players)
    {
        var slots = new List<ScheduleBookingOverview.TimeSlotItem>();
        for (var current = scheduleStart; current < scheduleEnd; current = current.AddMinutes(30))
        {
            var slotEnd = current.AddMinutes(30);
            var booking = court.Bookings
                .FirstOrDefault(b =>
                    !b.IsCancelled &&
                    b.ScheduleId == schedule.Id.GuidValue &&
                    b.TimeInterval.Start <= current &&
                    b.TimeInterval.End > current);

            ScheduleBookingOverview.BookingItem? bookingItem = null;
            if (booking != null)
            {
                var player = players.FirstOrDefault(p => p.Email.Equals(booking.PlayerEmail));
                var playerName = player == null
                    ? booking.PlayerEmail.Value
                    : $"{player.Name.FirstName} {player.Name.LastName}";

                bookingItem = new ScheduleBookingOverview.BookingItem(
                    booking.Id.ToString(),
                    playerName);
            }

            var isVip = vipIntervals.Any(v => current < v.End && slotEnd > v.Start);
            slots.Add(new ScheduleBookingOverview.TimeSlotItem(
                TimeOnly.FromDateTime(current),
                TimeOnly.FromDateTime(slotEnd),
                isVip,
                bookingItem));
        }

        return new ScheduleBookingOverview.CourtColumn(
            court.Id.ToString(),
            court.Id.ToString(),
            slots);
    }
}


