using System.Reflection;
using VIAPadelClub.Core.Domain.Aggregates.Courts;
using VIAPadelClub.Core.Domain.Aggregates.Courts.Entities;
using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Schedules;
using VIAPadelClub.Core.Domain.Services;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.ScheduleTests.RemoveSchedule;
file class NoConflictChecker : IScheduleDateConflictChecker
{
    public bool ActiveScheduleExistsOnDate(Guid excludeScheduleId, DateOnly date) => false;
}

public class RemoveAvailableCourtFromScheduleServiceTests
{
    private static readonly DateTime ScheduleDate = DateTime.Today.AddDays(1);
    private static readonly DateTime CurrentTime  = DateTime.Today;

    private static ViaEmail CreateEmail(string local = "123456")
        => ((Result<ViaEmail>.Success)ViaEmail.CreateEmail($"{local}@via.dk")).Value;

    private static Schedule CreateDraftSchedule()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(ScheduleDate);
        return schedule;
    }

    private static Schedule CreateDraftScheduleWithCourt(string courtName = "S1")
    {
        var schedule = CreateDraftSchedule();
        schedule.AddCourt(CourtId.Create(courtName).Payload);
        return schedule;
    }

    private static Schedule CreateActiveScheduleWithCourt(string courtName = "S1")
    {
        var schedule = CreateDraftSchedule();
        schedule.AddCourt(CourtId.Create(courtName).Payload);
        schedule.Activate(new NoConflictChecker());
        return schedule;
    }

    private static Court CreateCourt(string courtName = "S1") =>
        ((Result<Court>.Success)Court.Create(courtName)).Value;

    private static Court CreateCourtWithBooking(Guid scheduleId, ViaEmail playerEmail,
        DateTime? bookingStart = null, bool cancelled = false)
    {
        var court = CreateCourt();
        var start = bookingStart ?? ScheduleDate.AddHours(15);
        var timeInterval = TimeInterval.Create(start, start.AddHours(1)).Payload;

        typeof(Court)
            .GetMethod("AddBooking", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(court, [timeInterval, scheduleId, playerEmail]);

        if (cancelled)
        {
            var booking = court.Bookings[0];
            typeof(Booking)
                .GetMethod("Cancel", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(booking, null);
        }

        return court;
    }

    [Fact]
    public void Handle_CourtNotInSchedule_ReturnsFailure()
    {
        var schedule = CreateDraftScheduleWithCourt();
        var otherCourt = CreateCourt("S2");

        var result = RemoveAvailableCourtFromSchedule.Handle(schedule, otherCourt, CurrentTime);

        var failure = Assert.IsType<Result<IReadOnlyList<ViaEmail>>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("not found", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Handle_PastSchedule_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;
        schedule.AddCourt(CourtId.Create("S1").Payload);
        var court = CreateCourt();

        var result = RemoveAvailableCourtFromSchedule.Handle(schedule, court, CurrentTime.AddDays(1));

        var failure = Assert.IsType<Result<IReadOnlyList<ViaEmail>>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("past", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Handle_SameDayWithUpcomingBooking_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;
        schedule.AddCourt(CourtId.Create("S1").Payload);
        var playerEmail = CreateEmail();
        var court = CreateCourtWithBooking(schedule.Id, playerEmail,
            bookingStart: DateTime.Today.AddHours(20));

        var result = RemoveAvailableCourtFromSchedule.Handle(schedule, court, DateTime.Today.AddHours(10));

        var failure = Assert.IsType<Result<IReadOnlyList<ViaEmail>>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("upcoming", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Handle_DraftSchedule_ReturnsSuccess()
    {
        var schedule = CreateDraftScheduleWithCourt();
        var court = CreateCourt();

        var result = RemoveAvailableCourtFromSchedule.Handle(schedule, court, CurrentTime);

        Assert.IsType<Result<IReadOnlyList<ViaEmail>>.Success>(result);
    }

    [Fact]
    public void Handle_DraftSchedule_CourtIsRemoved()
    {
        var schedule = CreateDraftScheduleWithCourt();
        var court = CreateCourt();

        RemoveAvailableCourtFromSchedule.Handle(schedule, court, CurrentTime);

        Assert.DoesNotContain(court.Id, schedule.Courts);
    }

    [Fact]
    public void Handle_DraftSchedule_ReturnsEmptyEmailList()
    {
        var schedule = CreateDraftScheduleWithCourt();
        var court = CreateCourt();

        var result = RemoveAvailableCourtFromSchedule.Handle(schedule, court, CurrentTime);

        var success = Assert.IsType<Result<IReadOnlyList<ViaEmail>>.Success>(result);
        Assert.Empty(success.Value);
    }

    [Fact]
    public void Handle_ActiveScheduleBeforeDate_ReturnsSuccess()
    {
        var schedule = CreateActiveScheduleWithCourt();
        var court = CreateCourt();

        var result = RemoveAvailableCourtFromSchedule.Handle(schedule, court, CurrentTime);

        Assert.IsType<Result<IReadOnlyList<ViaEmail>>.Success>(result);
    }

    [Fact]
    public void Handle_ActiveScheduleWithBooking_BookingIsCancelled()
    {
        var schedule = CreateActiveScheduleWithCourt();
        var playerEmail = CreateEmail();
        var court = CreateCourtWithBooking(schedule.Id, playerEmail);

        RemoveAvailableCourtFromSchedule.Handle(schedule, court, CurrentTime);

        Assert.True(court.Bookings[0].IsCancelled);
    }

    [Fact]
    public void Handle_ActiveScheduleWithBooking_AffectedEmailReturned()
    {
        var schedule = CreateActiveScheduleWithCourt();
        var playerEmail = CreateEmail();
        var court = CreateCourtWithBooking(schedule.Id, playerEmail);

        var result = RemoveAvailableCourtFromSchedule.Handle(schedule, court, CurrentTime);

        var success = Assert.IsType<Result<IReadOnlyList<ViaEmail>>.Success>(result);
        Assert.Contains(success.Value, e => e == playerEmail);
    }

    [Fact]
    public void Handle_ActiveScheduleWithBooking_CourtIsRemoved()
    {
        var schedule = CreateActiveScheduleWithCourt();
        var playerEmail = CreateEmail();
        var court = CreateCourtWithBooking(schedule.Id, playerEmail);

        RemoveAvailableCourtFromSchedule.Handle(schedule, court, CurrentTime);

        Assert.DoesNotContain(court.Id, schedule.Courts);
    }

    [Fact]
    public void Handle_ActiveScheduleAlreadyCancelledBooking_NotIncludedInEmails()
    {
        var schedule = CreateActiveScheduleWithCourt();
        var playerEmail = CreateEmail();
        var court = CreateCourtWithBooking(schedule.Id, playerEmail, cancelled: true);

        var result = RemoveAvailableCourtFromSchedule.Handle(schedule, court, CurrentTime);

        var success = Assert.IsType<Result<IReadOnlyList<ViaEmail>>.Success>(result);
        Assert.Empty(success.Value);
    }

    [Fact]
    public void Handle_ActiveScheduleMultipleBookings_AllCancelledAndEmailsReturned()
    {
        var schedule = CreateActiveScheduleWithCourt();
        var email1 = CreateEmail("111111");
        var email2 = CreateEmail("222222");

        var court = CreateCourt();
        var interval1 = TimeInterval.Create(ScheduleDate.AddHours(15), ScheduleDate.AddHours(16)).Payload;
        var interval2 = TimeInterval.Create(ScheduleDate.AddHours(17), ScheduleDate.AddHours(18)).Payload;

        typeof(Court)
            .GetMethod("AddBooking", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(court, [interval1, schedule.Id, email1]);
        typeof(Court)
            .GetMethod("AddBooking", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(court, [interval2, schedule.Id, email2]);

        var result = RemoveAvailableCourtFromSchedule.Handle(schedule, court, CurrentTime);

        var success = Assert.IsType<Result<IReadOnlyList<ViaEmail>>.Success>(result);
        Assert.Equal(2, success.Value.Count);
        Assert.All(court.Bookings, b => Assert.True(b.IsCancelled));
    }

    [Fact]
    public void Handle_BookingOnDifferentSchedule_NotCancelledAndEmailNotReturned()
    {
        var schedule = CreateActiveScheduleWithCourt();
        var playerEmail = CreateEmail();
        var court = CreateCourtWithBooking(Guid.NewGuid(), playerEmail);

        var result = RemoveAvailableCourtFromSchedule.Handle(schedule, court, CurrentTime);

        var success = Assert.IsType<Result<IReadOnlyList<ViaEmail>>.Success>(result);
        Assert.Empty(success.Value);
        Assert.False(court.Bookings[0].IsCancelled);
    }
}