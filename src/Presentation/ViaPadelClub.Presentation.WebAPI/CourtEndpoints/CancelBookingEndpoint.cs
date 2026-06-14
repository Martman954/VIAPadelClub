using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.CourtCommands;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;
using ViaPadelClub.Presentation.WebAPI.Common;

namespace ViaPadelClub.Presentation.WebAPI.CourtEndpoints;

public sealed record CancelBookingRequest(string BookingId, string Email);

public class CancelBookingEndpoint
    : ApiEndpoint
        .WithRequest<CancelBookingRequest>
        .AndResults<Ok, BadRequest<IEnumerable<ResultError>>>
{
    [HttpPost("courts/bookings/cancel")]
    public override async Task<Results<Ok, BadRequest<IEnumerable<ResultError>>>> HandleAsync(
        CancelBookingRequest request,
        [FromServices] ICommandDispatcher dispatcher)
    {
        var commandResult = CancelBookingCommand.Create(request.BookingId, request.Email);
        if (commandResult is Result<CancelBookingCommand>.Failure validationFailure)
            return TypedResults.BadRequest(validationFailure.Errors);

        var result = await dispatcher.DispatchAsync(commandResult.Payload);
        if (result is Result<None>.Failure handlerFailure)
            return TypedResults.BadRequest(handlerFailure.Errors);

        return TypedResults.Ok();
    }
}

