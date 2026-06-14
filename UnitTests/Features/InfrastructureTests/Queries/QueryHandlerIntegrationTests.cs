using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VIAPadelClub.Core.Domain.Aggregates.Courts;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.ValueObjects;
using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Players;
using VIAPadelClub.Core.QueryContracts;
using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Infrastructure.EfcDomainPersistence;
using VIAPadelClub.Infrastructure.EfcQueries;

namespace UnitTests.Features.InfrastructureTests.Queries;

/// <summary>
/// Integration tests verifying that queries are dispatched and handlers return correct data from the DB.
/// </summary>
public class QueryHandlerIntegrationTests
{
    private const string ValidImageUrl = "https://via.dk/pic.png";

    // ─── Fixture helpers ─────────────────────────────────────────────────────

    private static ServiceProvider BuildServiceProvider(DomainModelContext context)
    {
        var services = new ServiceCollection();
        services.AddSingleton(context);
        services.AddEfcQueries();
        services.AddApplicationQueryDispatch(typeof(QueryHandlerIntegrationTests).Assembly);

        // Register the shared context as the DbContext EfcQueries handlers resolve
        services.AddScoped<DomainModelContext>(_ => context);

        return services.BuildServiceProvider();
    }

    private static DomainModelContext CreateDb() =>
        new(new DbContextOptionsBuilder<DomainModelContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static Player MakePlayer(string email, string first, string last)
    {
        var e = ((Result<ViaEmail>.Success)ViaEmail.CreateEmail(email)).Value;
        var n = ((Result<Name>.Success)Name.CreateName(first, last)).Value;
        var i = ((Result<ImageUrl>.Success)ImageUrl.CreateImageUrl(ValidImageUrl)).Value;
        return ((Result<Player>.Success)Player.Register(e, n, i, new AlwaysAvailableEmailChecker())).Value;
    }

    private static Court MakeCourt(string id = "S1") =>
        ((Result<Court>.Success)Court.Create(id)).Value;

    /// <summary>Creates a draft schedule on the given future date and returns it.</summary>
    private static Schedule MakeSchedule(DateTime futureDate, Court court)
    {
        var schedule = ((Result<Schedule>.Success)Schedule.Create()).Value;

        // Move the default time-slots to the future date
        schedule.UpdateDate(futureDate);

        var start = futureDate.Date.AddHours(15); // 15:00
        var end   = futureDate.Date.AddHours(17); // 17:00
        var interval = ((Result<TimeInterval>.Success)TimeInterval.Create(start, end)).Value;
        schedule.UpdateTimes(interval);

        schedule.AddCourt(court.Id);
        return schedule;
    }

    private static void AddBookingViaReflection(Court court, Guid scheduleId, ViaEmail playerEmail, DateTime start, DateTime end)
    {
        var interval = ((Result<TimeInterval>.Success)TimeInterval.Create(start, end)).Value;
        typeof(Court)
            .GetMethod("AddBooking", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(court, [interval, scheduleId, playerEmail]);
    }

    // ─── Tests ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task ManagerScheduleOverview_WhenScheduleExistsInMonth_ReturnsDayWithStatus()
    {
        // Arrange
        using var db = CreateDb();

        var court    = MakeCourt("S1");
        var schedule = MakeSchedule(new DateTime(2026, 8, 10), court);

        db.Add(court);
        db.Add(schedule);
        await db.SaveChangesAsync();

        await using var sp = BuildServiceProvider(db);
        var dispatcher = sp.GetRequiredService<IQueryDispatcher>();

        // Act
        var answer = await dispatcher.DispatchAsync(new ManagerScheduleOverview.Query(2026, 8));

        // Assert
        Assert.Equal(31, answer.Days.Count); // August has 31 days
        var day10 = answer.Days.Single(d => d.Date.Day == 10);
        Assert.NotNull(day10.Status); // schedule exists on that day
    }

    [Fact]
    public async Task ManagerScheduleOverview_WhenNoScheduleInMonth_ReturnsAllNullStatuses()
    {
        // Arrange
        using var db = CreateDb();
        await using var sp = BuildServiceProvider(db);
        var dispatcher = sp.GetRequiredService<IQueryDispatcher>();

        // Act
        var answer = await dispatcher.DispatchAsync(new ManagerScheduleOverview.Query(2026, 9));

        // Assert
        Assert.Equal(30, answer.Days.Count); // September has 30 days
        Assert.All(answer.Days, d => Assert.Null(d.Status));
    }

    [Fact]
    public async Task ScheduleBookingOverview_WhenScheduleHasTwoCourts_ReturnsBothCourtColumns()
    {
        // Arrange
        using var db = CreateDb();

        var court1   = MakeCourt("S1");
        var court2   = MakeCourt("S2");
        var schedule = MakeSchedule(new DateTime(2026, 8, 20), court1);
        schedule.AddCourt(court2.Id);

        db.Add(court1);
        db.Add(court2);
        db.Add(schedule);
        await db.SaveChangesAsync();

        await using var sp = BuildServiceProvider(db);
        var dispatcher = sp.GetRequiredService<IQueryDispatcher>();

        // Act
        var answer = await dispatcher.DispatchAsync(new ScheduleBookingOverview.Query(new DateOnly(2026, 8, 20)));

        // Assert
        Assert.Equal(2, answer.Courts.Count);
        Assert.All(answer.Courts, col => Assert.NotEmpty(col.Slots));
    }

    [Fact]
    public async Task ScheduleBookingOverview_WhenCourtHasBooking_SlotShowsPlayerName()
    {
        // Arrange
        using var db = CreateDb();

        var player   = MakePlayer("123456@via.dk", "Jane", "Smith");
        var court    = MakeCourt("S1");
        var futureDate = new DateTime(2026, 9, 5);
        var schedule = MakeSchedule(futureDate, court);
        var slotStart = futureDate.Date.AddHours(15);

        AddBookingViaReflection(court, schedule.Id.GuidValue, player.Email, slotStart, slotStart.AddMinutes(30));

        db.Add(player);
        db.Add(court);
        db.Add(schedule);
        await db.SaveChangesAsync();

        await using var sp = BuildServiceProvider(db);
        var dispatcher = sp.GetRequiredService<IQueryDispatcher>();

        // Act
        var answer = await dispatcher.DispatchAsync(new ScheduleBookingOverview.Query(new DateOnly(2026, 9, 5)));

        // Assert
        var bookedSlot = answer.Courts
            .SelectMany(c => c.Slots)
            .FirstOrDefault(s => s.Booking != null);

        Assert.NotNull(bookedSlot);
        Assert.Equal("Jane Smith", bookedSlot.Booking!.PlayerDisplayName);
    }

    [Fact]
    public async Task PlayerProfileOverview_WhenPlayerExists_ReturnsProfileWithCorrectName()
    {
        // Arrange
        using var db = CreateDb();

        var player = MakePlayer("123456@via.dk", "Alex", "Andersen");
        db.Add(player);
        await db.SaveChangesAsync();

        await using var sp = BuildServiceProvider(db);
        var dispatcher = sp.GetRequiredService<IQueryDispatcher>();

        // Act
        var answer = await dispatcher.DispatchAsync(new PlayerProfileOverview.Query("123456@via.dk"));

        // Assert
        Assert.Equal("Alex Andersen", answer.FullName);
        Assert.Equal("123456@via.dk", answer.Email);
        Assert.Equal(0, answer.UpcomingBookingCount);
    }

    [Fact]
    public async Task PlayerProfileOverview_WhenPlayerHasUpcomingBooking_ReturnsItInUpcomingList()
    {
        // Arrange
        using var db = CreateDb();

        var player    = MakePlayer("123456@via.dk", "Alex", "Andersen");
        var court     = MakeCourt("S1");
        var future    = new DateTime(2026, 10, 1);
        var schedule  = MakeSchedule(future, court);
        var slotStart = future.Date.AddHours(15);

        AddBookingViaReflection(court, schedule.Id.GuidValue, player.Email, slotStart, slotStart.AddMinutes(30));

        db.Add(player);
        db.Add(court);
        db.Add(schedule);
        await db.SaveChangesAsync();

        await using var sp = BuildServiceProvider(db);
        var dispatcher = sp.GetRequiredService<IQueryDispatcher>();

        // Act
        var answer = await dispatcher.DispatchAsync(new PlayerProfileOverview.Query("123456@via.dk"));

        // Assert
        Assert.Equal(1, answer.UpcomingBookingCount);
        Assert.Single(answer.UpcomingBookings);
        Assert.Empty(answer.PastBookings);
    }

    [Fact]
    public async Task PlayerQueueOverview_Always_ReturnsAnswer()
    {
        // Arrange – queue handler is a stub, no data needed
        using var db = CreateDb();
        await using var sp = BuildServiceProvider(db);
        var dispatcher = sp.GetRequiredService<IQueryDispatcher>();

        // Act
        var answer = await dispatcher.DispatchAsync(new PlayerQueueOverview.Query("123456@via.dk"));

        // Assert
        Assert.NotNull(answer);
        Assert.Empty(answer.Items);
    }
}

// ─── Local test helpers ───────────────────────────────────────────────────────

file sealed class AlwaysAvailableEmailChecker : IEmailInUseChecker
{
    public bool IsEmailInUse(ViaEmail email) => false;
}
