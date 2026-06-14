using Microsoft.AspNetCore.Mvc;
using VIAPadelClub.Core.QueryContracts;
using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Core.Tools.ObjectMapper;
using ViaPadelClub.Presentation.WebAPI.Common;

namespace ViaPadelClub.Presentation.WebAPI.QueryEndpoints;

public sealed class ScheduleBookingOverviewRequest
{
    public DateOnly Date { get; set; }
}

public sealed class ScheduleBookingOverviewResponse
{
    public DateOnly Date { get; set; }
    public IReadOnlyList<ScheduleBookingOverview.CourtColumn> Courts { get; set; } = [];
}

public sealed class ScheduleBookingOverviewEndpoint(IQueryDispatcher dispatcher, IObjectMapper mapper) : EndpointBase
{
    [HttpGet("schedules/{date}/bookings")]
    public async Task<ActionResult<ScheduleBookingOverviewResponse>> HandleAsync([FromRoute] ScheduleBookingOverviewRequest request)
    {
        var query = mapper.Map<ScheduleBookingOverviewRequest, ScheduleBookingOverview.Query>(request);
        var answer = await dispatcher.DispatchAsync(query);
        var response = mapper.Map<ScheduleBookingOverview.Answer, ScheduleBookingOverviewResponse>(answer);
        return Ok(response);
    }
}

