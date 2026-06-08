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

file class WithBookingsFinder : IPlayerBookingFinder
{
    private readonly Court _court;

    public WithBookingsFinder(Court court) => _court = court;

    public IReadOnlyList<Court> GetCourtsWithBookingsForPlayer(ViaEmail email) => [_court];
}

public class BlacklistPlayerServiceTests
{
    private static Player CreateTestPlayer()
    {
        var email = ((Result<ViaEmail>.Success)ViaEmail.CreateEmail("123456@via.dk")).Value;
        var name = ((Result<Name>.Success)Name.CreateName("Alex", "Andersen")).Value;
        var image = ((Result<ImageUrl>.Success)ImageUrl.CreateImageUrl("https://via.dk/pic.png")).Value;
        var result = Player.Register(email, name, image, new EmailAvailableChecker());
        return ((Result<Player>.Success)result).Value;
    }

    private static Court CreateCourtWithBooking(ViaEmail playerEmail)
    {
        var court = ((Result<Court>.Success)Court.Create("S1")).Value;

        var timeInterval = TimeInterval.Create(
            DateTime.Today.AddDays(1).AddHours(10),
            DateTime.Today.AddDays(1).AddHours(11)
        ).Payload;

        // AddBooking is internal — invoke via reflection
        typeof(Court)
            .GetMethod("AddBooking", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(court, [timeInterval, Guid.NewGuid(), playerEmail]);

        return court;
    }

    [Fact]
    public void Handle_WhenPlayerAlreadyBlacklisted_ReturnsFailure()
    {
        var player = CreateTestPlayer();
        typeof(Player)
            .GetMethod("Blacklist", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!
            .Invoke(player, null);

        var result = BlacklistPlayerService.Handle(player, new NoBookingsFinder());

        Assert.IsType<Result<None>.Failure>(result);
    }

    [Fact]
    public void Handle_WhenPlayerAlreadyBlacklisted_ErrorMentionsBlacklisted()
    {
        var player = CreateTestPlayer();
        typeof(Player)
            .GetMethod("Blacklist", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!
            .Invoke(player, null);

        var result = BlacklistPlayerService.Handle(player, new NoBookingsFinder());

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("blacklisted", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Handle_WhenPlayerHasNoBookings_ReturnsSuccess()
    {
        var player = CreateTestPlayer();

        var result = BlacklistPlayerService.Handle(player, new NoBookingsFinder());

        Assert.IsType<Result<None>.Success>(result);
    }

    [Fact]
    public void Handle_WhenPlayerHasNoBookings_PlayerIsBlacklisted()
    {
        var player = CreateTestPlayer();

        BlacklistPlayerService.Handle(player, new NoBookingsFinder());

        Assert.True(player.isBlackListed);
    }

    [Fact]
    public void Handle_WhenPlayerHasActiveBookings_BookingsAreCancelled()
    {
        var player = CreateTestPlayer();
        var court = CreateCourtWithBooking(player.Email);

        BlacklistPlayerService.Handle(player, new WithBookingsFinder(court));

        Assert.All(court.Bookings, b => Assert.True(b.IsCancelled));
    }

    [Fact]
    public void Handle_WhenPlayerHasActiveBookings_PlayerIsBlacklisted()
    {
        var player = CreateTestPlayer();
        var court = CreateCourtWithBooking(player.Email);

        BlacklistPlayerService.Handle(player, new WithBookingsFinder(court));

        Assert.True(player.isBlackListed);
    }

    [Fact]
    public void Handle_WhenPlayerHasAlreadyCancelledBooking_PlayerIsStillBlacklisted()
    {
        var player = CreateTestPlayer();
        var court = CreateCourtWithBooking(player.Email);
        
        var booking = court.Bookings[0];
        typeof(Booking)
            .GetMethod("Cancel", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(booking, null);

        BlacklistPlayerService.Handle(player, new WithBookingsFinder(court));

        Assert.True(player.isBlackListed);
    }

    [Fact]
    public void Handle_WhenPlayerHasAlreadyCancelledBooking_BookingRemainsOnlyCancelledOnce()
    {
        var player = CreateTestPlayer();
        var court = CreateCourtWithBooking(player.Email);

        var booking = court.Bookings[0];
        typeof(Booking)
            .GetMethod("Cancel", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(booking, null);

        BlacklistPlayerService.Handle(player, new WithBookingsFinder(court));
        
        Assert.True(booking.IsCancelled);
    }
}