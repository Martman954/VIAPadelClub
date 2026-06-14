using System.Reflection;
using VIAPadelClub.Core.Domain.Aggregates.Courts;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.ValueObjects;
using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Courts;
using VIAPadelClub.Core.Domain.Contracts.Players;
using VIAPadelClub.Core.Domain.Contracts.Schedules;
using VIAPadelClub.Core.Domain.Services;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.CourtTests.BookCourt;

file class EmailAvailableChecker : IEmailInUseChecker
{
    public bool IsEmailInUse(ViaEmail email) => false;
}

file class NoConflictChecker : IScheduleDateConflictChecker
{
    public bool ActiveScheduleExistsOnDate(ScheduleId excludeScheduleId, DateOnly date) => false;
}

file class NoExistingBookingChecker : ICourtHasBookingChecker
{
    public bool HasBooking(ViaEmail email, DateTime date) => false;
}

file class HasExistingBookingChecker : ICourtHasBookingChecker
{
    public bool HasBooking(ViaEmail email, DateTime date) => true;
}

public class BookCourtInScheduleServiceTests
{
    private static readonly DateTime ScheduleDate = DateTime.Today.AddDays(1);

    private static Player CreatePlayer()
    {
        var email = ((Result<ViaEmail>.Success)ViaEmail.CreateEmail("123456@via.dk")).Value;
        var name  = ((Result<Name>.Success)Name.CreateName("Alex", "Andersen")).Value;
        var image = ((Result<ImageUrl>.Success)ImageUrl.CreateImageUrl("https://via.dk/pic.png")).Value;
        return ((Result<Player>.Success)Player.Register(email, name, image, new EmailAvailableChecker())).Value;
    }

    private static (Schedule schedule, Court court) CreateActiveScheduleWithCourt()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(ScheduleDate);
        var courtId = CourtId.Create("S1").Payload;
        schedule.AddCourt(courtId);
        schedule.Activate(new NoConflictChecker());

        var court = ((Result<Court>.Success)Court.Create("S1")).Value;
        return (schedule, court);
    }

    private static TimeInterval ValidInterval()
        => TimeInterval.Create(ScheduleDate.AddHours(15), ScheduleDate.AddHours(16)).Payload;

    private static BookingRequest ValidRequest()
    {
        var player = CreatePlayer();
        var (schedule, court) = CreateActiveScheduleWithCourt();
        return new BookingRequest(player, court, schedule, ValidInterval());
    }

    [Fact]
    public void Handle_ValidRequest_ReturnsSuccess()
    {
        var result = BookCourtInSchedule.Handle(ValidRequest(), ScheduleDate.AddHours(-1), new NoExistingBookingChecker());

        Assert.IsType<Result<BookingId>.Success>(result);
    }

    [Fact]
    public void Handle_ValidRequest_BookingAppearsOnCourt()
    {
        var player = CreatePlayer();
        var (schedule, court) = CreateActiveScheduleWithCourt();
        var request = new BookingRequest(player, court, schedule, ValidInterval());

        BookCourtInSchedule.Handle(request, ScheduleDate.AddHours(-1), new NoExistingBookingChecker());

        Assert.Single(court.Bookings);
    }

    [Fact]
    public void Handle_ScheduleNotActive_ReturnsFailure()
    {
        var player = CreatePlayer();
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(ScheduleDate);
        var court = ((Result<Court>.Success)Court.Create("S1")).Value;
        var request = new BookingRequest(player, court, schedule, ValidInterval());

        var result = BookCourtInSchedule.Handle(request, ScheduleDate.AddHours(-1), new NoExistingBookingChecker());

        var failure = Assert.IsType<Result<BookingId>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("active", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Handle_CourtNotInSchedule_ReturnsFailure()
    {
        var player = CreatePlayer();
        var (schedule, _) = CreateActiveScheduleWithCourt();
        var otherCourt = ((Result<Court>.Success)Court.Create("S2")).Value;
        var request = new BookingRequest(player, otherCourt, schedule, ValidInterval());

        var result = BookCourtInSchedule.Handle(request, ScheduleDate.AddHours(-1), new NoExistingBookingChecker());

        var failure = Assert.IsType<Result<BookingId>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("not found", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Handle_BlacklistedPlayer_ReturnsFailure()
    {
        var player = CreatePlayer();
        typeof(Player)
            .GetMethod("Blacklist", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!
            .Invoke(player, null);

        var (schedule, court) = CreateActiveScheduleWithCourt();
        var request = new BookingRequest(player, court, schedule, ValidInterval());

        var result = BookCourtInSchedule.Handle(request, ScheduleDate.AddHours(-1), new NoExistingBookingChecker());

        var failure = Assert.IsType<Result<BookingId>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("blacklisted", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Handle_QuarantinedPlayer_ReturnsFailure()
    {
        var player = CreatePlayer();
        typeof(Player)
            .GetMethod("QuarantinePlayer", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!
            .Invoke(player, [ScheduleDate.AddHours(-1)]);

        var (schedule, court) = CreateActiveScheduleWithCourt();
        var request = new BookingRequest(player, court, schedule, ValidInterval());

        var result = BookCourtInSchedule.Handle(request, ScheduleDate.AddHours(-1), new NoExistingBookingChecker());

        var failure = Assert.IsType<Result<BookingId>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("quarantined", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Handle_PlayerAlreadyHasBookingOnDate_ReturnsFailure()
    {
        var request = ValidRequest();

        var result = BookCourtInSchedule.Handle(request, ScheduleDate.AddHours(-1), new HasExistingBookingChecker());

        var failure = Assert.IsType<Result<BookingId>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("one booking", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Handle_BookingStartsInPast_ReturnsFailure()
    {
        var player = CreatePlayer();
        var (schedule, court) = CreateActiveScheduleWithCourt();
        var pastInterval = TimeInterval.Create(ScheduleDate.AddHours(15), ScheduleDate.AddHours(16)).Payload;
        var request = new BookingRequest(player, court, schedule, pastInterval);

        var result = BookCourtInSchedule.Handle(request, ScheduleDate.AddHours(16), new NoExistingBookingChecker());

        var failure = Assert.IsType<Result<BookingId>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("past", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Handle_BookingNotOnHalfHour_ReturnsFailure()
    {
        var player = CreatePlayer();
        var (schedule, court) = CreateActiveScheduleWithCourt();
        var badInterval = TimeInterval.Create(ScheduleDate.AddHours(15).AddMinutes(15), ScheduleDate.AddHours(16).AddMinutes(15)).Payload;
        var request = new BookingRequest(player, court, schedule, badInterval);

        var result = BookCourtInSchedule.Handle(request, ScheduleDate.AddHours(-1), new NoExistingBookingChecker());

        Assert.IsType<Result<BookingId>.Failure>(result);
    }

    [Fact]
    public void Handle_BookingLessThanOneHour_ReturnsFailure()
    {
        var player = CreatePlayer();
        var (schedule, court) = CreateActiveScheduleWithCourt();
        var shortInterval = TimeInterval.Create(ScheduleDate.AddHours(15), ScheduleDate.AddHours(15).AddMinutes(30)).Payload;
        var request = new BookingRequest(player, court, schedule, shortInterval);

        var result = BookCourtInSchedule.Handle(request, ScheduleDate.AddHours(-1), new NoExistingBookingChecker());

        var failure = Assert.IsType<Result<BookingId>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("1 hour", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Handle_BookingMoreThanThreeHours_ReturnsFailure()
    {
        var player = CreatePlayer();
        var (schedule, court) = CreateActiveScheduleWithCourt();
        var longInterval = TimeInterval.Create(ScheduleDate.AddHours(15), ScheduleDate.AddHours(19)).Payload;
        var request = new BookingRequest(player, court, schedule, longInterval);

        var result = BookCourtInSchedule.Handle(request, ScheduleDate.AddHours(-1), new NoExistingBookingChecker());

        var failure = Assert.IsType<Result<BookingId>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("3 hours", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Handle_BookingOutsideScheduleBounds_ReturnsFailure()
    {
        var player = CreatePlayer();
        var (schedule, court) = CreateActiveScheduleWithCourt();
        var outOfBounds = TimeInterval.Create(ScheduleDate.AddHours(21), ScheduleDate.AddHours(23)).Payload;
        var request = new BookingRequest(player, court, schedule, outOfBounds);

        var result = BookCourtInSchedule.Handle(request, ScheduleDate.AddHours(-1), new NoExistingBookingChecker());

        Assert.IsType<Result<BookingId>.Failure>(result);
    }

    [Fact]
    public void Handle_NonVipPlayerBookingVipSlot_ReturnsFailure()
    {
        var player = CreatePlayer();
        var (_, court) = CreateActiveScheduleWithCourt();
        
        var draftSchedule = Schedule.Create().Payload;
        draftSchedule.UpdateDate(ScheduleDate);
        var noOverlapChecker = new NoOverlapCheckerStub();
        var vipInterval = TimeInterval.Create(ScheduleDate.AddHours(15), ScheduleDate.AddHours(16)).Payload;
        draftSchedule.MarkVipTimeSpan(vipInterval, noOverlapChecker);
        draftSchedule.AddCourt(CourtId.Create("S1").Payload);
        draftSchedule.Activate(new NoConflictChecker());

        var request = new BookingRequest(player, court, draftSchedule, ValidInterval());

        var result = BookCourtInSchedule.Handle(request, ScheduleDate.AddHours(-1), new NoExistingBookingChecker());

        var failure = Assert.IsType<Result<BookingId>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("VIP", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Handle_VipPlayerBookingVipSlot_ReturnsSuccess()
    {
        var player = CreatePlayer();
        player.ElevateToVip(ScheduleDate.AddHours(-1));

        var draftSchedule = Schedule.Create().Payload;
        draftSchedule.UpdateDate(ScheduleDate);
        var vipInterval = TimeInterval.Create(ScheduleDate.AddHours(15), ScheduleDate.AddHours(16)).Payload;
        draftSchedule.MarkVipTimeSpan(vipInterval, new NoOverlapCheckerStub());
        draftSchedule.AddCourt(CourtId.Create("S1").Payload);
        draftSchedule.Activate(new NoConflictChecker());

        var court = ((Result<Court>.Success)Court.Create("S1")).Value;
        var request = new BookingRequest(player, court, draftSchedule, ValidInterval());

        var result = BookCourtInSchedule.Handle(request, ScheduleDate.AddHours(-1), new NoExistingBookingChecker());

        Assert.IsType<Result<BookingId>.Success>(result);
    }
    

    [Fact]
    public void Handle_OverlappingBooking_ReturnsFailure()
    {
        var player = CreatePlayer();
        var (schedule, court) = CreateActiveScheduleWithCourt();
        
        typeof(Court)
            .GetMethod("AddBooking", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(court, [ValidInterval(), schedule.Id.GuidValue, player.Email]);

        var player2Email = ((Result<ViaEmail>.Success)ViaEmail.CreateEmail("654321@via.dk")).Value;
        var player2Name  = ((Result<Name>.Success)Name.CreateName("Bob", "Builder")).Value;
        var player2Image = ((Result<ImageUrl>.Success)ImageUrl.CreateImageUrl("https://via.dk/pic2.png")).Value;
        var player2 = ((Result<Player>.Success)Player.Register(player2Email, player2Name, player2Image, new EmailAvailableChecker())).Value;

        var request = new BookingRequest(player2, court, schedule, ValidInterval());

        var result = BookCourtInSchedule.Handle(request, ScheduleDate.AddHours(-1), new NoExistingBookingChecker());

        var failure = Assert.IsType<Result<BookingId>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("not available", StringComparison.OrdinalIgnoreCase));
    }
}

file class NoOverlapCheckerStub : INonVipBookingOverlapChecker
{
    public bool HasNonVipBookingsInTimeSpan(ScheduleId scheduleId, TimeInterval timeInterval) => false;
}