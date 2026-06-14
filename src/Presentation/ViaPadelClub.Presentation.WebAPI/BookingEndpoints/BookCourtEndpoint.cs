using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.CourtCommands;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using ViaPadelClub.Presentation.WebAPI.Common;

namespace ViaPadelClub.Presentation.WebAPI.BookingEndpoints;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


public class BookCourtEndpoint(ICommandHandler<BookCourtCommand> handler)
    : ApiEndpoint.WithRequest<BookCourtEndpoint.BookCourtRequest>.AndResult<IResult>
{
    [HttpPost("courts/book")]
    public override async Task<IResult> HandleAsync(BookCourtRequest request)
    {
        // 1. Build the command — validation lives here
        var commandResult = BookCourtCommand.Create(
            request.PlayerId,
            request.CourtId,
            request.ScheduleId,
            request.StartTime,
            request.EndTime);

        if (commandResult is Result<BookCourtCommand>.Failure cf)
            return Results.BadRequest(cf.Errors);

        // 2. Dispatch to the application handler
        var result = await handler.HandleAsync(commandResult.Payload);

        // 3. Translate the Result into an HTTP response
        if (result is Result<None>.Failure rf)
            return Results.BadRequest(rf.Errors);

        return Results.Ok();
    }


    public record BookCourtRequest(
        string PlayerId,
        string CourtId,
        string ScheduleId,
        DateTime StartTime,
        DateTime EndTime
    );
}