using Microsoft.AspNetCore.Mvc;
using VIAPadelClub.Core.QueryContracts;
using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Core.Tools.ObjectMapper;
using ViaPadelClub.Presentation.WebAPI.Common;

namespace ViaPadelClub.Presentation.WebAPI.QueryEndpoints;

public sealed class PlayerProfileOverviewRequest
{
    public string PlayerIdOrEmail { get; set; } = string.Empty;
}

public sealed class PlayerProfileOverviewResponse
{
    public string PlayerId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? VipUntilText { get; set; }
    public int UpcomingBookingCount { get; set; }
    public IReadOnlyList<PlayerProfileOverview.BookingCard> UpcomingBookings { get; set; } = [];
    public IReadOnlyList<PlayerProfileOverview.BookingCard> PastBookings { get; set; } = [];
}

public sealed class PlayerProfileOverviewEndpoint(IQueryDispatcher dispatcher, IObjectMapper mapper) : EndpointBase
{
    [HttpGet("players/{playerIdOrEmail}/profile")]
    public async Task<ActionResult<PlayerProfileOverviewResponse>> HandleAsync([FromRoute] PlayerProfileOverviewRequest request)
    {
        var query = mapper.Map<PlayerProfileOverviewRequest, PlayerProfileOverview.Query>(request);
        var answer = await dispatcher.DispatchAsync(query);
        var response = mapper.Map<PlayerProfileOverview.Answer, PlayerProfileOverviewResponse>(answer);
        return Ok(response);
    }
}

