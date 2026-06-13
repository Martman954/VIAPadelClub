using VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;
using VIAPadelClub.Core.Application.Features.Schedules;
using VIAPadelClub.Core.Domain.Aggregates.Schedules.Enums;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using ScheduleAggregate = VIAPadelClub.Core.Domain.Aggregates.Schedules.Schedule;

namespace UnitTests.Features.ApplicationTests.Schedule;

file class FakeScheduleRepo : IScheduleRepo
{
    public List<ScheduleAggregate> Schedules { get; } = [];

    public Task<ScheduleAggregate> AddSchedule(ScheduleAggregate schedule)
    {
        Schedules.Add(schedule);
        return Task.FromResult(schedule);
    }

    public Task<ScheduleAggregate> GetSchedule(Guid scheduleId)
        => Task.FromResult(Schedules.First(s => s.Id == scheduleId));

    public Task<ScheduleAggregate> RemoveSchedule(Guid scheduleId)
    {
        var schedule = Schedules.First(s => s.Id == scheduleId);
        Schedules.Remove(schedule);
        return Task.FromResult(schedule);
    }
}

file class FakeUnitOfWork : IUnitOfWork
{
    public bool SaveChangesCalled { get; private set; }

    public Task SaveChangesAsync()
    {
        SaveChangesCalled = true;
        return Task.CompletedTask;
    }
}

public class CreateScheduleHandlerTests
{
    private static CreateScheduleCommand ValidCommand()
        => new CreateScheduleCommand();

    [Fact]
    public async Task HandleAsync_ValidCommand_ReturnsSuccess()
    {
        var repo = new FakeScheduleRepo();
        var uow  = new FakeUnitOfWork();
        var handler = new CreateScheduleHandler(repo, uow);

        var result = await handler.HandleAsync(ValidCommand());

        Assert.IsType<Result<None>.Success>(result);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_ScheduleIsAddedToRepo()
    {
        var repo = new FakeScheduleRepo();
        var uow  = new FakeUnitOfWork();
        var handler = new CreateScheduleHandler(repo, uow);

        await handler.HandleAsync(ValidCommand());

        Assert.Single(repo.Schedules);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_ScheduleIsInDraftStatus()
    {
        var repo = new FakeScheduleRepo();
        var uow  = new FakeUnitOfWork();
        var handler = new CreateScheduleHandler(repo, uow);

        await handler.HandleAsync(ValidCommand());

        Assert.Equal(Status.Draft, repo.Schedules[0].Status);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_SaveChangesIsCalled()
    {
        var repo = new FakeScheduleRepo();
        var uow  = new FakeUnitOfWork();
        var handler = new CreateScheduleHandler(repo, uow);

        await handler.HandleAsync(ValidCommand());

        Assert.True(uow.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_ScheduleHasNoCoruts()
    {
        var repo = new FakeScheduleRepo();
        var uow  = new FakeUnitOfWork();
        var handler = new CreateScheduleHandler(repo, uow);

        await handler.HandleAsync(ValidCommand());

        Assert.Empty(repo.Schedules[0].Courts);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_ScheduleHasDefaultTimes()
    {
        var repo = new FakeScheduleRepo();
        var uow  = new FakeUnitOfWork();
        var handler = new CreateScheduleHandler(repo, uow);

        await handler.HandleAsync(ValidCommand());

        var schedule = repo.Schedules[0];
        Assert.Equal(new TimeOnly(15, 0), TimeOnly.FromDateTime(schedule.Times[0].TimeInterval.Start));
        Assert.Equal(new TimeOnly(22, 0), TimeOnly.FromDateTime(schedule.Times[0].TimeInterval.End));
    }
    
}