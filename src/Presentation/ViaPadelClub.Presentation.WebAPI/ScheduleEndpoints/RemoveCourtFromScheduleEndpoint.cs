using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;
using ViaPadelClub.Presentation.WebAPI.Common;

namespace ViaPadelClub.Presentation.WebAPI.ScheduleEndpoints;

public sealed record RemoveCourtFromScheduleRequest(string ScheduleId, string CourtId);

public class RemoveCourtFromScheduleEndpoint
    : ApiEndpoint
        .WithRequest<RemoveCourtFromScheduleRequest>
        .AndResults<Ok, BadRequest<IEnumerable<ResultError>>>
{
    [HttpPost("schedules/courts/remove")]
    public override async Task<Results<Ok, BadRequest<IEnumerable<ResultError>>>> HandleAsync(
        RemoveCourtFromScheduleRequest request,
        [FromServices] ICommandDispatcher dispatcher)
    {
        var commandResult = RemoveCourtFromScheduleCommand.Create(request.ScheduleId, request.CourtId);
        if (commandResult is Result<RemoveCourtFromScheduleCommand>.Failure validationFailure)
            return TypedResults.BadRequest(validationFailure.Errors);

        var result = await dispatcher.DispatchAsync(commandResult.Payload);
        if (result is Result<None>.Failure handlerFailure)
            return TypedResults.BadRequest(handlerFailure.Errors);

        return TypedResults.Ok();
    }
}

