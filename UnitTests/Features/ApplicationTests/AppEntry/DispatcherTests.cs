using Microsoft.Extensions.DependencyInjection;
using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using ScheduleAggregate = VIAPadelClub.Core.Domain.Aggregates.Schedules.Schedule;

namespace UnitTests.Features.ApplicationTests.AppEntry;

file sealed record PingCommand(string Value);

file sealed class PingSpy
{
    public bool WasHandled { get; set; }
    public string? LastValue { get; set; }
}

file sealed class PingHandler(PingSpy spy) : ICommandHandler<PingCommand>
{
    public Task<Result> HandleAsync(PingCommand command)
    {
        spy.WasHandled = true;
        spy.LastValue = command.Value;
        return Task.FromResult<Result>(Result.Success());
    }
}

file sealed class FakeScheduleRepo : IScheduleRepository
{
    public List<ScheduleAggregate> Schedules { get; } = [];

    public Task AddAsync(ScheduleAggregate schedule)
    {
        Schedules.Add(schedule);
        return Task.CompletedTask;
    }

    public Task<ScheduleAggregate?> GetAsync(ScheduleId scheduleId)
        => Task.FromResult(Schedules.FirstOrDefault(s => s.Id.Equals(scheduleId)));

    public Task RemoveAsync(ScheduleId scheduleId)
    {
        var schedule = Schedules.FirstOrDefault(s => s.Id.Equals(scheduleId));
        if (schedule != null)
            Schedules.Remove(schedule);
        return Task.CompletedTask;
    }
}

file sealed class FakeUnitOfWork : IUnitOfWork
{
    public bool SaveChangesCalled { get; private set; }

    public Task SaveChangesAsync()
    {
        SaveChangesCalled = true;
        return Task.CompletedTask;
    }
}

file sealed class StubDispatcher(Result nextResult) : ICommandDispatcher
{
    public Task<Result> DispatchAsync<TCommand>(TCommand command)
        => Task.FromResult(nextResult);
}

public class DispatcherTests
{
    [Fact]
    public async Task AutoTransactionSubmitDispatcher_WhenSuccess_CallsSaveChanges()
    {
        var uow = new FakeUnitOfWork();
        var next = new StubDispatcher(Result.Success());
        var sut = new AutoTransactionSubmitDispatcher(next, uow);

        var result = await sut.DispatchAsync(new PingCommand("ok"));

        Assert.IsType<Result<None>.Success>(result);
        Assert.True(uow.SaveChangesCalled);
    }

    [Fact]
    public async Task AutoTransactionSubmitDispatcher_WhenFailure_DoesNotCallSaveChanges()
    {
        var uow = new FakeUnitOfWork();
        var next = new StubDispatcher(Result.Failure("boom", VIAPadelClub.Core.Tools.OperationResult.Results.Errors.ErrorType.Failure));
        var sut = new AutoTransactionSubmitDispatcher(next, uow);

        var result = await sut.DispatchAsync(new PingCommand("fail"));

        Assert.IsType<Result<None>.Failure>(result);
        Assert.False(uow.SaveChangesCalled);
    }

    [Fact]
    public async Task DispatchAsync_WhenHandlerRegistered_InvokesMatchingHandler()
    {
        var services = new ServiceCollection();
        services.AddScoped<ICommandDispatcher, Dispatcher>();
        services.AddScoped<ICommandHandler<PingCommand>, PingHandler>();
        services.AddScoped<PingSpy>();

        await using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var dispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();
        var spy = scope.ServiceProvider.GetRequiredService<PingSpy>();

        var result = await dispatcher.DispatchAsync(new PingCommand("hello"));

        Assert.IsType<Result<None>.Success>(result);
        Assert.True(spy.WasHandled);
        Assert.Equal("hello", spy.LastValue);
    }

    [Fact]
    public async Task DispatchAsync_WhenHandlerMissing_ThrowsInvalidOperationException()
    {
        var services = new ServiceCollection();
        services.AddScoped<ICommandDispatcher, Dispatcher>();

        await using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var dispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => dispatcher.DispatchAsync(new PingCommand("missing")));

        Assert.Contains("No command handler is registered", ex.Message);
    }

    [Fact]
    public async Task AddApplicationCommandDispatch_WhenConfigured_ResolvesCreateScheduleHandler()
    {
        var services = new ServiceCollection();
        services.AddApplicationCommandDispatch();
        services.AddScoped<IScheduleRepository, FakeScheduleRepo>();
        services.AddScoped<IUnitOfWork, FakeUnitOfWork>();

        await using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var dispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();
        var repo = (FakeScheduleRepo)scope.ServiceProvider.GetRequiredService<IScheduleRepository>();
        var uow = (FakeUnitOfWork)scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var result = await dispatcher.DispatchAsync(new CreateScheduleCommand());

        Assert.IsType<Result<None>.Success>(result);
        Assert.Single(repo.Schedules);
        Assert.True(uow.SaveChangesCalled);
    }
}




