using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;
using ViaPadelClub.Presentation.WebAPI.Common;

namespace ViaPadelClub.Presentation.WebAPI.ScheduleEndpoints;

public sealed record UpdateScheduleDateTimeRequest(
    string ScheduleId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime);

public class UpdateScheduleDateTimeEndpoint
    : ApiEndpoint
        .WithRequest<UpdateScheduleDateTimeRequest>
        .AndResults<Ok, BadRequest<IEnumerable<ResultError>>>
{
    [HttpPost("schedules/update-datetime")]
    public override async Task<Results<Ok, BadRequest<IEnumerable<ResultError>>>> HandleAsync(
        UpdateScheduleDateTimeRequest request,
        [FromServices] ICommandDispatcher dispatcher)
    {
        var commandResult = UpdateScheduleDateTimeCommand.Create(
            request.ScheduleId,
            request.Date,
            request.StartTime,
            request.EndTime);

        if (commandResult is Result<UpdateScheduleDateTimeCommand>.Failure validationFailure)
            return TypedResults.BadRequest(validationFailure.Errors);

        var result = await dispatcher.DispatchAsync(commandResult.Payload);
        if (result is Result<None>.Failure handlerFailure)
            return TypedResults.BadRequest(handlerFailure.Errors);

        return TypedResults.Ok();
    }
}
