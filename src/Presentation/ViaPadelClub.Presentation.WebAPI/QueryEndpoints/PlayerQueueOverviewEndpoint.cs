using Microsoft.AspNetCore.Mvc;
using VIAPadelClub.Core.QueryContracts;
using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Core.Tools.ObjectMapper;
using ViaPadelClub.Presentation.WebAPI.Common;

namespace ViaPadelClub.Presentation.WebAPI.QueryEndpoints;

public sealed class PlayerQueueOverviewRequest
{
    public string PlayerIdOrEmail { get; set; } = string.Empty;
}

public sealed class PlayerQueueOverviewResponse
{
    public IReadOnlyList<PlayerQueueOverview.QueueItem> Items { get; set; } = [];
}

public sealed class PlayerQueueOverviewEndpoint(IQueryDispatcher dispatcher, IObjectMapper mapper) : EndpointBase
{
    [HttpGet("players/{playerIdOrEmail}/queue")]
    public async Task<ActionResult<PlayerQueueOverviewResponse>> HandleAsync([FromRoute] PlayerQueueOverviewRequest request)
    {
        var query = mapper.Map<PlayerQueueOverviewRequest, PlayerQueueOverview.Query>(request);
        var answer = await dispatcher.DispatchAsync(query);
        var response = mapper.Map<PlayerQueueOverview.Answer, PlayerQueueOverviewResponse>(answer);
        return Ok(response);
    }
}

