using System.Reflection;
using VIAPadelClub.Core.Domain.Aggregates.Courts;
using VIAPadelClub.Core.Domain.Aggregates.Courts.Entities;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.ValueObjects;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Players;
using VIAPadelClub.Core.Domain.Services;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.ServiceTests;

file class EmailAvailableChecker : IEmailInUseChecker
{
    public bool IsEmailInUse(ViaEmail email) => false;
}

file class NoBookingsFinder : IPlayerBookingFinder
{
    public IReadOnlyList<Court> GetCourtsWithBookingsForPlayer(ViaEmail email) => [];
}

file class WithBookingsFinder(params Court[] courts) : IPlayerBookingFinder
{
    private readonly IReadOnlyList<Court> _courts = courts;
    public IReadOnlyList<Court> GetCourtsWithBookingsForPlayer(ViaEmail email) => _courts;
}

public class QuarantinePlayerServiceTests
{
    private static readonly DateTime CurrentTime = DateTime.Today;
    
    private static readonly DateTime WithinQuarantine  = CurrentTime.AddDays(1);
    private static readonly DateTime OutsideQuarantine = CurrentTime.AddDays(4);

    private static Player CreatePlayer()
    {
        var email = ((Result<ViaEmail>.Success)ViaEmail.CreateEmail("123456@via.dk")).Value;
        var name  = ((Result<Name>.Success)Name.CreateName("Alex", "Andersen")).Value;
        var image = ((Result<ImageUrl>.Success)ImageUrl.CreateImageUrl("https://via.dk/pic.png")).Value;
        return ((Result<Player>.Success)Player.Register(email, name, image, new EmailAvailableChecker())).Value;
    }

    private static Court CreateCourtWithBooking(ViaEmail playerEmail, DateTime bookingStart)
    {
        var court = ((Result<Court>.Success)Court.Create("S1")).Value;
        var timeInterval = TimeInterval.Create(bookingStart, bookingStart.AddHours(1)).Payload;
        typeof(Court)
            .GetMethod("AddBooking", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(court, [timeInterval, Guid.NewGuid(), playerEmail]);
        return court;
    }

    private static void PreCancelBooking(Court court)
    {
        var booking = court.Bookings[0];
        typeof(Booking)
            .GetMethod("Cancel", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(booking, null);
    }

    [Fact]
    public void Handle_NormalPlayer_ReturnsSuccess()
    {
        var player = CreatePlayer();

        var result = QuarantinePlayerService.Handle(player, CurrentTime, new NoBookingsFinder());

        Assert.IsType<Result<None>.Success>(result);
    }

    [Fact]
    public void Handle_NormalPlayer_PlayerIsQuarantined()
    {
        var player = CreatePlayer();

        QuarantinePlayerService.Handle(player, CurrentTime, new NoBookingsFinder());

        Assert.True(player.IsQuarantined(CurrentTime));
    }

    [Fact]
    public void Handle_BookingWithinQuarantinePeriod_IsCancelled()
    {
        var player = CreatePlayer();
        var court = CreateCourtWithBooking(player.Email, WithinQuarantine);

        QuarantinePlayerService.Handle(player, CurrentTime, new WithBookingsFinder(court));

        Assert.True(court.Bookings[0].IsCancelled);
    }

    [Fact]
    public void Handle_BookingOutsideQuarantinePeriod_IsNotCancelled()
    {
        var player = CreatePlayer();
        var court = CreateCourtWithBooking(player.Email, OutsideQuarantine);

        QuarantinePlayerService.Handle(player, CurrentTime, new WithBookingsFinder(court));

        Assert.False(court.Bookings[0].IsCancelled);
    }

    [Fact]
    public void Handle_AlreadyCancelledBookingWithinQuarantine_RemainsUnchanged()
    {
        var player = CreatePlayer();
        var court = CreateCourtWithBooking(player.Email, WithinQuarantine);
        PreCancelBooking(court);

        QuarantinePlayerService.Handle(player, CurrentTime, new WithBookingsFinder(court));

        Assert.True(court.Bookings[0].IsCancelled);
    }

    [Fact]
    public void Handle_MultipleBookings_OnlyCancelsThoseWithinQuarantine()
    {
        var player = CreatePlayer();
        var courtInside  = CreateCourtWithBooking(player.Email, WithinQuarantine);
        var courtOutside = CreateCourtWithBooking(player.Email, OutsideQuarantine);

        QuarantinePlayerService.Handle(player, CurrentTime, new WithBookingsFinder(courtInside, courtOutside));

        Assert.True(courtInside.Bookings[0].IsCancelled);
        Assert.False(courtOutside.Bookings[0].IsCancelled);
    }

    [Fact]
    public void Handle_AlreadyQuarantinedPlayer_ReturnsSuccess()
    {
        var player = CreatePlayer();
        QuarantinePlayerService.Handle(player, CurrentTime, new NoBookingsFinder());

        var result = QuarantinePlayerService.Handle(player, CurrentTime, new NoBookingsFinder());

        Assert.IsType<Result<None>.Success>(result);
    }

    [Fact]
    public void Handle_AlreadyQuarantinedPlayer_QuarantineIsExtended()
    {
        var player = CreatePlayer();
        QuarantinePlayerService.Handle(player, CurrentTime, new NoBookingsFinder());
        
        QuarantinePlayerService.Handle(player, CurrentTime, new NoBookingsFinder());

        Assert.True(player.IsQuarantined(CurrentTime.AddDays(6)));
        Assert.False(player.IsQuarantined(CurrentTime.AddDays(7)));
    }
}