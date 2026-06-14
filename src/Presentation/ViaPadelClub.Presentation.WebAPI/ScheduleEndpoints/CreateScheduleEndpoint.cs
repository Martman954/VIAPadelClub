using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;
using ViaPadelClub.Presentation.WebAPI.Common;

namespace ViaPadelClub.Presentation.WebAPI.ScheduleEndpoints;

public class CreateScheduleEndpoint
    : ApiEndpoint
        .WithoutRequest
        .AndResults<Ok, BadRequest<IEnumerable<ResultError>>>
{
    [HttpPost("schedules")]
    public override async Task<Results<Ok, BadRequest<IEnumerable<ResultError>>>> HandleAsync(
        [FromServices] ICommandDispatcher dispatcher)
    {
        var result = await dispatcher.DispatchAsync(new CreateScheduleCommand());
        if (result is Result<None>.Failure handlerFailure)
            return TypedResults.BadRequest(handlerFailure.Errors);

        return TypedResults.Ok();
    }
}

