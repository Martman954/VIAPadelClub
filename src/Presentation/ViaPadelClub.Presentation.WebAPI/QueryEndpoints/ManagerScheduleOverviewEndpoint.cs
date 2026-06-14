using Microsoft.AspNetCore.Mvc;
using VIAPadelClub.Core.QueryContracts;
using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Core.Tools.ObjectMapper;
using ViaPadelClub.Presentation.WebAPI.Common;

namespace ViaPadelClub.Presentation.WebAPI.QueryEndpoints;

public sealed class ManagerScheduleOverviewRequest
{
    public int Year { get; set; }
    public int Month { get; set; }
}

public sealed class ManagerScheduleOverviewResponse
{
    public int Year { get; set; }
    public int Month { get; set; }
    public IReadOnlyList<ManagerScheduleOverview.DayStatusItem> Days { get; set; } = [];
}

public sealed class ManagerScheduleOverviewEndpoint(IQueryDispatcher dispatcher, IObjectMapper mapper) : EndpointBase
{
    [HttpGet("schedules/{year:int}/{month:int}/overview")]
    public async Task<ActionResult<ManagerScheduleOverviewResponse>> HandleAsync([FromRoute] ManagerScheduleOverviewRequest request)
    {
        var query = mapper.Map<ManagerScheduleOverviewRequest, ManagerScheduleOverview.Query>(request);
        var answer = await dispatcher.DispatchAsync(query);
        var response = mapper.Map<ManagerScheduleOverview.Answer, ManagerScheduleOverviewResponse>(answer);
        return Ok(response);
    }
}

