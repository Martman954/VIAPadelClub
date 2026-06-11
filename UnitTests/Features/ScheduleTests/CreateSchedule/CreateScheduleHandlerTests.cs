using Features.CommandDispatch.ScheduleCommands;
using Features.Features.Schedule;
using VIAPadelClub.Core.Domain.Aggregates.Schedules.Enums;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.ApplicationTests.Schedule;

file class FakeScheduleRepo : IScheduleRepo
{
    public List<VIAPadelClub.Core.Domain.Aggregates.Schedules.Schedule> Schedules { get; } = [];

    public Task<VIAPadelClub.Core.Domain.Aggregates.Schedules.Schedule> AddSchedule(VIAPadelClub.Core.Domain.Aggregates.Schedules.Schedule schedule)
    {
        Schedules.Add(schedule);
        return Task.FromResult(schedule);
    }

    public Task<VIAPadelClub.Core.Domain.Aggregates.Schedules.Schedule> GetSchedule(Guid scheduleId)
        => Task.FromResult(Schedules.First(s => s.Id == scheduleId));

    public Task<VIAPadelClub.Core.Domain.Aggregates.Schedules.Schedule> RemoveSchedule(Guid scheduleId)
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
        => ((Result<CreateScheduleCommand>.Success)
            CreateScheduleCommand.Create("Morning Session", Status.Draft)).Value;

    [Fact]
    public void CreateCommand_ValidInputs_ReturnsSuccess()
    {
        var result = CreateScheduleCommand.Create("Morning Session", Status.Draft);

        Assert.IsType<Result<CreateScheduleCommand>.Success>(result);
    }

    [Fact]
    public void CreateCommand_EmptyTitle_ReturnsFailure()
    {
        var result = CreateScheduleCommand.Create("", Status.Draft);

        Assert.IsType<Result<CreateScheduleCommand>.Failure>(result);
    }

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
    
    [Fact]
    public void CreateCommand_NullTitle_ThrowsOrReturnsFailure()
    {
        Assert.Throws<NullReferenceException>(() => CreateScheduleCommand.Create(null!, Status.Draft));
    }
}