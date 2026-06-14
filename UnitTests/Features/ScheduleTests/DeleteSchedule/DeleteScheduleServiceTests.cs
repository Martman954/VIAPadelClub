using System.Reflection;
using VIAPadelClub.Core.Domain.Aggregates.Courts;
using VIAPadelClub.Core.Domain.Aggregates.Courts.Entities;
using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Services;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.ScheduleTests.DeleteSchedule;

public class DeleteScheduleServiceTests
{
    private static readonly DateTime ScheduleDate = DateTime.Today.AddDays(1);
    private static readonly DateTime CurrentTime = DateTime.Today;

    private static Schedule CreateFutureSchedule()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(ScheduleDate);
        return schedule;
    }

    private static ViaEmail CreateEmail(string email)
        => ((Result<ViaEmail>.Success)ViaEmail.CreateEmail(email)).Value;

    private static Court CreateCourtWithBooking(Guid scheduleId, ViaEmail playerEmail, bool cancelled = false)
    {
        var court = ((Result<Court>.Success)Court.Create("S1")).Value;

        var timeInterval = TimeInterval.Create(
            ScheduleDate.AddHours(15),
            ScheduleDate.AddHours(16)
        ).Payload;

        var bookingIdResult = (Result<BookingId>.Success)typeof(Court)
            .GetMethod("AddBooking", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(court, [timeInterval, scheduleId, playerEmail])!;

        if (cancelled)
        {
            var booking = court.Bookings.First(b => b.Id == bookingIdResult.Value);
            typeof(Booking)
                .GetMethod("Cancel", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(booking, null);
        }

        return court;
    }
    

    [Fact]
    public void Handle_AlreadyDeletedSchedule_ReturnsFailure()
    {
        var schedule = CreateFutureSchedule();
        typeof(Schedule)
            .GetMethod("Delete", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(schedule, [CurrentTime]);

        var result = DeleteScheduleService.Handle(schedule, [], CurrentTime);

        Assert.IsType<Result<IReadOnlyList<ViaEmail>>.Failure>(result);
    }

    [Fact]
    public void Handle_PastSchedule_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload; // defaults to today

        var result = DeleteScheduleService.Handle(schedule, [], CurrentTime.AddDays(1));

        var failure = Assert.IsType<Result<IReadOnlyList<ViaEmail>>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("past", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Handle_ScheduleOnSameDay_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload; // defaults to today

        var result = DeleteScheduleService.Handle(schedule, [], CurrentTime);

        var failure = Assert.IsType<Result<IReadOnlyList<ViaEmail>>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("same date", StringComparison.OrdinalIgnoreCase));
    }
    

    [Fact]
    public void Handle_FutureScheduleNoCourts_ReturnsSuccess()
    {
        var schedule = CreateFutureSchedule();

        var result = DeleteScheduleService.Handle(schedule, [], CurrentTime);

        Assert.IsType<Result<IReadOnlyList<ViaEmail>>.Success>(result);
    }

    [Fact]
    public void Handle_FutureScheduleNoCourts_ReturnsEmptyEmailList()
    {
        var schedule = CreateFutureSchedule();

        var result = DeleteScheduleService.Handle(schedule, [], CurrentTime);

        var success = Assert.IsType<Result<IReadOnlyList<ViaEmail>>.Success>(result);
        Assert.Empty(success.Value);
    }

    [Fact]
    public void Handle_FutureScheduleWithBooking_BookingIsCancelled()
    {
        var schedule = CreateFutureSchedule();
        var playerEmail = CreateEmail("123456@via.dk");
        var court = CreateCourtWithBooking(schedule.Id.GuidValue, playerEmail);

        DeleteScheduleService.Handle(schedule, [court], CurrentTime);

        Assert.All(court.Bookings, b => Assert.True(b.IsCancelled));
    }

    [Fact]
    public void Handle_FutureScheduleWithBooking_AffectedEmailIsReturned()
    {
        var schedule = CreateFutureSchedule();
        var playerEmail = CreateEmail("123456@via.dk");
        var court = CreateCourtWithBooking(schedule.Id.GuidValue, playerEmail);

        var result = DeleteScheduleService.Handle(schedule, [court], CurrentTime);

        var success = Assert.IsType<Result<IReadOnlyList<ViaEmail>>.Success>(result);
        Assert.Contains(success.Value, e => e == playerEmail);
    }

    [Fact]
    public void Handle_MultipleBookings_AllCancelledAndEmailsReturned()
    {
        var schedule = CreateFutureSchedule();
        var email1 = CreateEmail("123456@via.dk");
        var email2 = CreateEmail("654321@via.dk");

        var court1 = CreateCourtWithBooking(schedule.Id.GuidValue, email1);
        var court2 = CreateCourtWithBooking(schedule.Id.GuidValue, email2);

        var result = DeleteScheduleService.Handle(schedule, [court1, court2], CurrentTime);

        var success = Assert.IsType<Result<IReadOnlyList<ViaEmail>>.Success>(result);
        Assert.Equal(2, success.Value.Count);
        Assert.True(court1.Bookings[0].IsCancelled);
        Assert.True(court2.Bookings[0].IsCancelled);
    }

    [Fact]
    public void Handle_AlreadyCancelledBooking_NotIncludedInAffectedEmails()
    {
        var schedule = CreateFutureSchedule();
        var playerEmail = CreateEmail("123456@via.dk");
        var court = CreateCourtWithBooking(schedule.Id.GuidValue, playerEmail, cancelled: true);

        var result = DeleteScheduleService.Handle(schedule, [court], CurrentTime);

        var success = Assert.IsType<Result<IReadOnlyList<ViaEmail>>.Success>(result);
        Assert.Empty(success.Value);
    }

    [Fact]
    public void Handle_BookingOnDifferentSchedule_NotCancelled()
    {
        var schedule = CreateFutureSchedule();
        var playerEmail = CreateEmail("123456@via.dk");
        var court = CreateCourtWithBooking(Guid.NewGuid(), playerEmail);

        DeleteScheduleService.Handle(schedule, [court], CurrentTime);

        Assert.False(court.Bookings[0].IsCancelled);
    }
}