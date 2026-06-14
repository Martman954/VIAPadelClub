using System.Net;
using System.Net.Http.Json;
using ViaPadelClub.Presentation.WebAPI.QueryEndpoints;

namespace UnitTests.Endpoints;

public class QueryEndpointsIntegrationTests : IClassFixture<EndpointIntegrationFactory>
{
    private readonly HttpClient _client;

    public QueryEndpointsIntegrationTests(EndpointIntegrationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetManagerScheduleOverview_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/schedules/2026/6/overview");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetPlayerProfileOverview_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/players/abcd@via.dk/profile");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetScheduleBookingOverview_ReturnsOk()
    {
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)).ToString("yyyy-MM-dd");
        var response = await _client.GetAsync($"/api/schedules/{date}/bookings");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

