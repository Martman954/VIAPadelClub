using Microsoft.AspNetCore.Http.HttpResults;
using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.CourtCommands;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;
using ViaPadelClub.Presentation.WebAPI.BookingEndpoints;

namespace UnitTests.Endpoints;

public class BookCourtEndpointTests
{
    // Dispatcher stub we fully control — returns whatever Result we hand it.
    private sealed class StubDispatcher(Result result) : ICommandDispatcher
    {
        public bool WasCalled { get; private set; }

        public Task<Result> DispatchAsync<TCommand>(TCommand command)
        {
            WasCalled = true;
            return Task.FromResult(result);
        }
    }

    // Plug in values that pass ViaEmail / CourtId / TimeInterval validators.
    private static BookCourtRequest ValidRequest() => new(
        PlayerId:   "abcd@via.dk",
        CourtId:    "S1",
        ScheduleId: Guid.NewGuid().ToString(),
        StartTime:  new DateTime(2026, 12, 1, 10, 0, 0),
        EndTime:    new DateTime(2026, 12, 1, 11, 0, 0));

    [Fact]
    public async Task InvalidInput_ReturnsBadRequest_AndDoesNotCallDispatcher()
    {
        var dispatcher = new StubDispatcher(Result.Success());
        var endpoint = new BookCourtEndpoint();

        // Empty player id fails ViaEmail validation, so Create() fails.
        var request = ValidRequest() with { PlayerId = "" };

        var result = await endpoint.HandleAsync(request, dispatcher);

        Assert.IsType<BadRequest<IEnumerable<ResultError>>>(result.Result);
        Assert.False(dispatcher.WasCalled); // short-circuited before dispatch
    }

    [Fact]
    public async Task ValidInput_DispatcherFails_ReturnsBadRequest()
    {
        var dispatcher = new StubDispatcher(
            Result.Failure("Court not available.", ErrorType.Validation));
        var endpoint = new BookCourtEndpoint();

        var result = await endpoint.HandleAsync(ValidRequest(), dispatcher);

        Assert.IsType<BadRequest<IEnumerable<ResultError>>>(result.Result);
        Assert.True(dispatcher.WasCalled);
    }

    [Fact]
    public async Task ValidInput_DispatcherSucceeds_ReturnsOk()
    {
        var dispatcher = new StubDispatcher(Result.Success());
        var endpoint = new BookCourtEndpoint();

        var result = await endpoint.HandleAsync(ValidRequest(), dispatcher);

        Assert.IsType<Ok>(result.Result);
        Assert.True(dispatcher.WasCalled);
    }
}