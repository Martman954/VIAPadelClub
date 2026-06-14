using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.PlayerCommands;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;
using ViaPadelClub.Presentation.WebAPI.Common;

namespace ViaPadelClub.Presentation.WebAPI.PlayerEndpoints;

public sealed record RegisterAsNewPlayerRequest(
    string Email,
    string FirstName,
    string LastName,
    string ImageUrl);

public class RegisterAsNewPlayerEndpoint
    : ApiEndpoint
        .WithRequest<RegisterAsNewPlayerRequest>
        .AndResults<Ok, BadRequest<IEnumerable<ResultError>>>
{
    [HttpPost("players")]
    public override async Task<Results<Ok, BadRequest<IEnumerable<ResultError>>>> HandleAsync(
        RegisterAsNewPlayerRequest request,
        [FromServices] ICommandDispatcher dispatcher)
    {
        var commandResult = RegisterAsNewPlayerCommand.Create(
            request.Email,
            request.FirstName,
            request.LastName,
            request.ImageUrl);

        if (commandResult is Result<RegisterAsNewPlayerCommand>.Failure validationFailure)
            return TypedResults.BadRequest(validationFailure.Errors);

        var result = await dispatcher.DispatchAsync(commandResult.Payload);
        if (result is Result<None>.Failure handlerFailure)
            return TypedResults.BadRequest(handlerFailure.Errors);

        return TypedResults.Ok();
    }
}

