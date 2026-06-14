using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;
using ViaPadelClub.Presentation.WebAPI.Common;

namespace ViaPadelClub.Presentation.WebAPI.ScheduleEndpoints;

public sealed record AddCourtToScheduleRequest(string ScheduleId, string CourtId);

public class AddCourtToScheduleEndpoint
    : ApiEndpoint
        .WithRequest<AddCourtToScheduleRequest>
        .AndResults<Ok, BadRequest<IEnumerable<ResultError>>>
{
    [HttpPost("schedules/courts/add")]
    public override async Task<Results<Ok, BadRequest<IEnumerable<ResultError>>>> HandleAsync(
        AddCourtToScheduleRequest request,
        [FromServices] ICommandDispatcher dispatcher)
    {
        var commandResult = AddCourtToScheduleCommand.Create(request.ScheduleId, request.CourtId);
        if (commandResult is Result<AddCourtToScheduleCommand>.Failure validationFailure)
            return TypedResults.BadRequest(validationFailure.Errors);

        var result = await dispatcher.DispatchAsync(commandResult.Payload);
        if (result is Result<None>.Failure handlerFailure)
            return TypedResults.BadRequest(handlerFailure.Errors);

        return TypedResults.Ok();
    }
}

