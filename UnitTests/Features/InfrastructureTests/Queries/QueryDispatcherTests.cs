using Microsoft.Extensions.DependencyInjection;
using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.QueryContracts;

namespace UnitTests.Features.InfrastructureTests.Queries;

file sealed record PingQuery(string Value) : IQuery<PingAnswer>;
file sealed record PingAnswer(string Value);

file sealed class PingHandler : IQueryHandler<PingQuery, PingAnswer>
{
    public Task<PingAnswer> HandleAsync(PingQuery query)
        => Task.FromResult(new PingAnswer(query.Value));
}

public class QueryDispatcherTests
{
    [Fact]
    public async Task DispatchAsync_WhenHandlerRegistered_ReturnsAnswer()
    {
        var services = new ServiceCollection();
        services.AddScoped<IQueryHandler<PingQuery, PingAnswer>, PingHandler>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        await using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var dispatcher = scope.ServiceProvider.GetRequiredService<IQueryDispatcher>();

        var answer = await dispatcher.DispatchAsync(new PingQuery("hello"));

        Assert.Equal("hello", answer.Value);
    }

    [Fact]
    public async Task DispatchAsync_WhenHandlerMissing_ThrowsInvalidOperationException()
    {
        var services = new ServiceCollection();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        await using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var dispatcher = scope.ServiceProvider.GetRequiredService<IQueryDispatcher>();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => dispatcher.DispatchAsync(new PingQuery("missing")));

        Assert.Contains("No query handler is registered", ex.Message);
    }
}


