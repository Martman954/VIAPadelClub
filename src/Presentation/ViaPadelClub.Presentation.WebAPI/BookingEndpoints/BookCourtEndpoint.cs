using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.CourtCommands;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;
using ViaPadelClub.Presentation.WebAPI.Common;

namespace ViaPadelClub.Presentation.WebAPI.BookingEndpoints;

public record BookCourtRequest(
    string PlayerId,
    string CourtId,
    string ScheduleId,
    DateTime StartTime,
    DateTime EndTime);

public class BookCourtEndpoint
    : ApiEndpoint
        .WithRequest<BookCourtRequest>
        .AndResults<Ok, BadRequest<IEnumerable<ResultError>>>
{
    [HttpPost("courts/book")]
    public override async Task<Results<Ok, BadRequest<IEnumerable<ResultError>>>> HandleAsync(
        BookCourtRequest request,
        [FromServices] ICommandDispatcher dispatcher)
    {
        // 1. Build & validate the command
        var commandResult = BookCourtCommand.Create(
            request.PlayerId,
            request.CourtId,
            request.ScheduleId,
            request.StartTime,
            request.EndTime);

        if (commandResult is Result<BookCourtCommand>.Failure validationFailure)
            return TypedResults.BadRequest(validationFailure.Errors);

        // 2. Dispatch to the application layer
        var result = await dispatcher.DispatchAsync(commandResult.Payload);

        // 3. Translate the application Result into an HTTP response
        if (result is Result<None>.Failure handlerFailure)
            return TypedResults.BadRequest(handlerFailure.Errors);

        return TypedResults.Ok();
    }
}