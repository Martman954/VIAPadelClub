using System.Reflection;
using VIAPadelClub.Core.Domain.Aggregates.Courts;
using VIAPadelClub.Core.Domain.Aggregates.Courts.Entities;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.CourtTests;

public class CourtCancelBookingAggregateTests
{
    private static readonly DateTime FutureStart = DateTime.Today.AddDays(1).AddHours(15);

    private static Court CreateCourt() =>
        ((Result<Court>.Success)Court.Create("S1")).Value;

    private static ViaEmail CreateEmail() =>
        ((Result<ViaEmail>.Success)ViaEmail.CreateEmail("123456@via.dk")).Value;

    private static BookingId AddBooking(Court court, DateTime start, DateTime end)
    {
        var interval = TimeInterval.Create(start, end).Payload;
        return ((Result<BookingId>.Success)typeof(Court)
            .GetMethod("AddBooking", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(court, [interval, Guid.NewGuid(), CreateEmail()])!).Value;
    }

    private static void PreCancel(Court court, BookingId id)
    {
        var booking = court.Bookings.First(b => b.Id == id);
        typeof(Booking)
            .GetMethod("Cancel", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(booking, null);
    }

    [Fact]
    public void CancelBooking_ValidFutureBooking_ReturnsSuccess()
    {
        var court = CreateCourt();
        var id = AddBooking(court, FutureStart, FutureStart.AddHours(1));

        var result = court.CancelBooking(id, DateTime.Today);

        Assert.IsType<Result<None>.Success>(result);
    }

    [Fact]
    public void CancelBooking_ValidFutureBooking_BookingIsCancelled()
    {
        var court = CreateCourt();
        var id = AddBooking(court, FutureStart, FutureStart.AddHours(1));

        court.CancelBooking(id, DateTime.Today);

        Assert.True(court.Bookings.First(b => b.Id == id).IsCancelled);
    }

    [Fact]
    public void CancelBooking_NonExistentId_ReturnsFailure()
    {
        var court = CreateCourt();

        var result = court.CancelBooking(BookingId.New(), DateTime.Today);

        Assert.IsType<Result<None>.Failure>(result);
    }

    [Fact]
    public void CancelBooking_NonExistentId_ErrorMentionsNotFound()
    {
        var court = CreateCourt();

        var result = court.CancelBooking(BookingId.New(), DateTime.Today);

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("not found", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void CancelBooking_AlreadyCancelled_ReturnsFailure()
    {
        var court = CreateCourt();
        var id = AddBooking(court, FutureStart, FutureStart.AddHours(1));
        PreCancel(court, id);

        var result = court.CancelBooking(id, DateTime.Today);

        Assert.IsType<Result<None>.Failure>(result);
    }

    [Fact]
    public void CancelBooking_AlreadyCancelled_ErrorMentionsCancelled()
    {
        var court = CreateCourt();
        var id = AddBooking(court, FutureStart, FutureStart.AddHours(1));
        PreCancel(court, id);

        var result = court.CancelBooking(id, DateTime.Today);

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("cancelled", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void CancelBooking_PastBooking_ReturnsFailure()
    {
        var court = CreateCourt();
        var pastStart = DateTime.Today.AddDays(-1).AddHours(10);
        var id = AddBooking(court, pastStart, pastStart.AddHours(1));

        var result = court.CancelBooking(id, DateTime.Today);

        Assert.IsType<Result<None>.Failure>(result);
    }

    [Fact]
    public void CancelBooking_PastBooking_ErrorMentionsPast()
    {
        var court = CreateCourt();
        var pastStart = DateTime.Today.AddDays(-1).AddHours(10);
        var id = AddBooking(court, pastStart, pastStart.AddHours(1));

        var result = court.CancelBooking(id, DateTime.Today);

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("past", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void CancelBooking_WithinOneHourOfStart_ReturnsFailure()
    {
        var court = CreateCourt();
        var start = DateTime.Today.AddHours(14);
        var id = AddBooking(court, start, start.AddHours(1));
        var currentTime = DateTime.Today.AddHours(13).AddMinutes(30);

        var result = court.CancelBooking(id, currentTime);

        Assert.IsType<Result<None>.Failure>(result);
    }

    [Fact]
    public void CancelBooking_WithinOneHourOfStart_ErrorMentionsOneHour()
    {
        var court = CreateCourt();
        var start = DateTime.Today.AddHours(14);
        var id = AddBooking(court, start, start.AddHours(1));
        var currentTime = DateTime.Today.AddHours(13).AddMinutes(30);

        var result = court.CancelBooking(id, currentTime);

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("one hour", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void CancelBooking_ExactlyOneHourBeforeStart_ReturnsSuccess()
    {
        var court = CreateCourt();
        var start = DateTime.Today.AddHours(14);
        var id = AddBooking(court, start, start.AddHours(1));
        var currentTime = DateTime.Today.AddHours(13);

        var result = court.CancelBooking(id, currentTime);

        Assert.IsType<Result<None>.Success>(result);
    }
}