using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Domain.Aggregates.Courts;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.QueryContracts;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Infrastructure.EfcDomainPersistence;

namespace VIAPadelClub.Infrastructure.EfcQueries;

public sealed class PlayerProfileOverviewQueryHandler(DomainModelContext context)
    : IQueryHandler<PlayerProfileOverview.Query, PlayerProfileOverview.Answer>
{
    public async Task<PlayerProfileOverview.Answer> HandleAsync(PlayerProfileOverview.Query query)
    {
        var players = await context.Set<Player>().ToListAsync();
        var courts = await context.Set<Court>().ToListAsync();
        var schedules = await context.Set<Schedule>().ToListAsync();

        var emailResult = ViaEmail.CreateEmail(query.PlayerIdOrEmail);
        if (emailResult is not Result<ViaEmail>.Success valid)
        {
            return new PlayerProfileOverview.Answer(
                query.PlayerIdOrEmail,
                string.Empty,
                query.PlayerIdOrEmail,
                null,
                0,
                [],
                []);
        }

        var player = players.FirstOrDefault(p => p.Id.Equals(valid.Value));
        if (player == null)
        {
            return new PlayerProfileOverview.Answer(
                valid.Value.ToString(),
                string.Empty,
                valid.Value.Value,
                null,
                0,
                [],
                []);
        }

        var bookings = courts
            .SelectMany(court => court.Bookings.Select(booking => new { court, booking }))
            .Where(x =>
                !x.booking.IsCancelled &&
                x.booking.PlayerEmail.Equals(player.Email) &&
                TryGetScheduleDate(schedules, x.booking.ScheduleId, out _))
            .Select(x =>
            {
                _ = TryGetScheduleDate(schedules, x.booking.ScheduleId, out var scheduleDate);
                return new
                {
                    x.booking,
                    x.court,
                    Date = scheduleDate
                };
            })
            .OrderBy(x => x.booking.TimeInterval.Start)
            .ToList();

        var now = DateTime.Now;

        var upcoming = bookings
            .Where(x => x.booking.TimeInterval.Start >= now)
            .Select(x => ToBookingCard(x.booking, x.court, x.Date))
            .ToList();

        var past = bookings
            .Where(x => x.booking.TimeInterval.Start < now)
            .Select(x => ToBookingCard(x.booking, x.court, x.Date))
            .ToList();

        var vipUntil = player.VipStatus?.EndDate.ToString("yyyy-MM-dd");

        return new PlayerProfileOverview.Answer(
            player.Id.ToString(),
            $"{player.Name.FirstName} {player.Name.LastName}",
            player.Email.Value,
            vipUntil,
            upcoming.Count,
            upcoming,
            past);
    }

    private static bool TryGetScheduleDate(
        IReadOnlyList<Schedule> schedules,
        Guid scheduleId,
        out DateOnly date)
    {
        var schedule = schedules.FirstOrDefault(s => s.Id.GuidValue == scheduleId && s.Times.Count > 0);
        if (schedule == null)
        {
            date = default;
            return false;
        }

        date = DateOnly.FromDateTime(schedule.Times.Min(t => t.TimeInterval.Start));
        return true;
    }

    private static PlayerProfileOverview.BookingCard ToBookingCard(
        VIAPadelClub.Core.Domain.Aggregates.Courts.Entities.Booking booking,
        Court court,
        DateOnly date)
    {
        return new PlayerProfileOverview.BookingCard(
            booking.Id.ToString(),
            date,
            TimeOnly.FromDateTime(booking.TimeInterval.Start),
            TimeOnly.FromDateTime(booking.TimeInterval.End),
            court.Id.ToString());
    }
}


