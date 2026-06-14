using Microsoft.AspNetCore.Http.HttpResults;
using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.CourtCommands;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;
using ViaPadelClub.Presentation.WebAPI.BookingEndpoints;

namespace UnitTests.Endpoints;

public class BookCourtEndpointTests
{
    // A stub handler we fully control — returns whatever Result we hand it.
    private sealed class StubHandler(Result result) : ICommandHandler<BookCourtCommand>
    {
        public bool WasCalled { get; private set; }

        public Task<Result> HandleAsync(BookCourtCommand command)
        {
            WasCalled = true;
            return Task.FromResult(result);
        }
    }

    // Plug in values that actually pass your ViaEmail / CourtId / TimeInterval validators.
    private static BookCourtRequest ValidRequest() => new(
        PlayerId:   "abcd@via.dk",
        CourtId:    "Court-1",
        ScheduleId: Guid.NewGuid().ToString(),
        StartTime:  new DateTime(2025, 1, 1, 10, 0, 0),
        EndTime:    new DateTime(2025, 1, 1, 11, 0, 0));

    [Fact]
    public async Task InvalidInput_ReturnsBadRequest_AndDoesNotCallHandler()
    {
        var handler = new StubHandler(Result.Success());
        var endpoint = new BookCourtEndpoint(handler);

        // Empty player id fails ViaEmail validation, so Create() fails.
        var request = ValidRequest() with { PlayerId = "" };

        var result = await endpoint.HandleAsync(request);

        Assert.IsType<BadRequest<IEnumerable<ResultError>>>(result.Result);
        Assert.False(handler.WasCalled); // short-circuited before dispatch
    }

    [Fact]
    public async Task ValidInput_HandlerFails_ReturnsBadRequest()
    {
        var handler = new StubHandler(
            Result.Failure("Court not available.", ErrorType.Validation));
        var endpoint = new BookCourtEndpoint(handler);

        var result = await endpoint.HandleAsync(ValidRequest());

        Assert.IsType<BadRequest<IEnumerable<ResultError>>>(result.Result);
        Assert.True(handler.WasCalled);
    }

    [Fact]
    public async Task ValidInput_HandlerSucceeds_ReturnsOk()
    {
        var handler = new StubHandler(Result.Success());
        var endpoint = new BookCourtEndpoint(handler);

        var result = await endpoint.HandleAsync(ValidRequest());

        Assert.IsType<Ok>(result.Result);
        Assert.True(handler.WasCalled);
    }
}