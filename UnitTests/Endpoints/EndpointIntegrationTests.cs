using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using VIAPadelClub.Core.Application.ExternalServices;
using VIAPadelClub.Core.Domain.Aggregates.Courts;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Courts;
using VIAPadelClub.Core.Domain.Contracts.Players;
using VIAPadelClub.Core.Domain.Contracts.Schedules;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;

namespace UnitTests.Endpoints;

public class EndpointIntegrationTests : IClassFixture<EndpointIntegrationFactory>
{
    private readonly HttpClient _client;

    public EndpointIntegrationTests(EndpointIntegrationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RegisterPlayer_WithInvalidPayload_ReturnsBadRequest()
    {
        var payload = new
        {
            Email = "not-an-email",
            FirstName = "",
            LastName = "",
            ImageUrl = "not-a-url"
        };

        var response = await _client.PostAsJsonAsync("/api/players", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddCourtToSchedule_WithInvalidIds_ReturnsBadRequest()
    {
        var payload = new
        {
            ScheduleId = "not-a-guid",
            CourtId = "invalid"
        };

        var response = await _client.PostAsJsonAsync("/api/schedules/courts/add", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateSchedule_ReturnsOk()
    {
        var response = await _client.PostAsync("/api/schedules", content: null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task RegisterPlayer_WithValidPayload_ReturnsOk()
    {
        var payload = new
        {
            Email = "abcd@via.dk",
            FirstName = "Alex",
            LastName = "Andersen",
            ImageUrl = "https://via.dk/pic.png"
        };

        var response = await _client.PostAsJsonAsync("/api/players", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AddCourtToSchedule_WithValidPayload_ReturnsOk()
    {
        var payload = new
        {
            ScheduleId = Guid.NewGuid().ToString(),
            CourtId = "S1"
        };

        var response = await _client.PostAsJsonAsync("/api/schedules/courts/add", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

public sealed class EndpointIntegrationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddScoped<IScheduleRepository, FakeScheduleRepository>();
            services.AddScoped<IPlayerRepository, FakePlayerRepository>();
            services.AddScoped<ICourtRepository, FakeCourtRepository>();

            services.AddScoped<IBookingCourtFinder, FakeBookingCourtFinder>();
            services.AddScoped<ICourtHasBookingChecker, FakeCourtHasBookingChecker>();
            services.AddScoped<IScheduleDateConflictChecker, FakeScheduleDateConflictChecker>();
            services.AddScoped<IEmailInUseChecker, FakeEmailInUseChecker>();

            services.AddScoped<ICourtRemovalNotifier, FakeCourtRemovalNotifier>();
            services.AddScoped<IUnitOfWork, FakeUnitOfWork>();
        });
    }
}

file sealed class FakeScheduleRepository : IScheduleRepository
{
    public Task<Schedule?> GetAsync(ScheduleId scheduleId)
    {
        var schedule = ((VIAPadelClub.Core.Tools.OperationResult.Results.Result<Schedule>.Success)Schedule.Create()).Value;
        _ = schedule.UpdateDate(DateTime.Today.AddDays(1));
        return Task.FromResult<Schedule?>(schedule);
    }

    public Task AddAsync(Schedule aggregate) => Task.CompletedTask;
    public Task RemoveAsync(ScheduleId id) => Task.CompletedTask;
}

file sealed class FakePlayerRepository : IPlayerRepository
{
    public Task<Player?> GetAsync(ViaEmail playerId) => Task.FromResult<Player?>(null);
    public Task AddAsync(Player aggregate) => Task.CompletedTask;
    public Task RemoveAsync(ViaEmail id) => Task.CompletedTask;
}

file sealed class FakeCourtRepository : ICourtRepository
{
    public Task<Court?> GetAsync(CourtId courtId) => Task.FromResult<Court?>(null);
    public Task AddAsync(Court aggregate) => Task.CompletedTask;
    public Task RemoveAsync(CourtId id) => Task.CompletedTask;
}

file sealed class FakeBookingCourtFinder : IBookingCourtFinder
{
    public Task<Court?> FindCourtWithBooking(BookingId bookingId) => Task.FromResult<Court?>(null);
}

file sealed class FakeCourtHasBookingChecker : ICourtHasBookingChecker
{
    public bool HasBooking(ViaEmail email, DateTime date) => false;
}

file sealed class FakeScheduleDateConflictChecker : IScheduleDateConflictChecker
{
    public bool ActiveScheduleExistsOnDate(ScheduleId excludeScheduleId, DateOnly date) => false;
}

file sealed class FakeEmailInUseChecker : IEmailInUseChecker
{
    public bool IsEmailInUse(ViaEmail email) => false;
}

file sealed class FakeCourtRemovalNotifier : ICourtRemovalNotifier
{
    public Task NotifyCourtRemovedAsync(IReadOnlyList<ViaEmail> affectedEmails, Guid scheduleId, CourtId courtId)
        => Task.CompletedTask;
}

file sealed class FakeUnitOfWork : IUnitOfWork
{
    public Task SaveChangesAsync() => Task.CompletedTask;
}
