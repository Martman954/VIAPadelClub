using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Application.ExternalServices;
using VIAPadelClub.Core.Domain.Aggregates.Courts;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Aggregates.Schedules.Enums;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Courts;
using VIAPadelClub.Core.Domain.Contracts.Players;
using VIAPadelClub.Core.Domain.Contracts.Schedules;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Infrastructure.EfcDomainPersistence;

namespace ViaPadelClub.Presentation.WebAPI.Common;

internal sealed class EfUnitOfWork(DomainModelContext context) : IUnitOfWork
{
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}

internal sealed class EfEmailInUseChecker(DomainModelContext context) : IEmailInUseChecker
{
    public bool IsEmailInUse(ViaEmail email)
        => context.Set<Player>().Any(p => p.Id == email);
}

internal sealed class EfScheduleDateConflictChecker(DomainModelContext context) : IScheduleDateConflictChecker
{
    public bool ActiveScheduleExistsOnDate(ScheduleId excludeScheduleId, DateOnly date)
    {
        var schedules = context.Set<Schedule>().ToList();
        return schedules.Any(s =>
            !s.Id.Equals(excludeScheduleId)
            && s.Status == Status.Active
            && s.Times.Count > 0
            && DateOnly.FromDateTime(s.Times.Min(t => t.TimeInterval.Start)) == date);
    }
}

internal sealed class EfCourtHasBookingChecker(DomainModelContext context) : ICourtHasBookingChecker
{
    public bool HasBooking(ViaEmail email, DateTime date)
    {
        var courts = context.Set<Court>().ToList();
        return courts.SelectMany(c => c.Bookings)
            .Any(b => !b.IsCancelled && b.PlayerEmail.Equals(email) && b.TimeInterval.Start.Date == date.Date);
    }
}

internal sealed class EfBookingCourtFinder(DomainModelContext context) : IBookingCourtFinder
{
    public Task<Court?> FindCourtWithBooking(BookingId bookingId)
    {
        var court = context.Set<Court>()
            .ToList()
            .FirstOrDefault(c => c.Bookings.Any(b => b.Id == bookingId));

        return Task.FromResult(court);
    }
}

internal sealed class NoOpCourtRemovalNotifier : ICourtRemovalNotifier
{
    public Task NotifyCourtRemovedAsync(IReadOnlyList<ViaEmail> affectedEmails, Guid scheduleId, CourtId courtId)
        => Task.CompletedTask;
}

