using Features.CommandDispatch.ScheduleCommands;
using Features.Features.Schedule;
using UnitTests.Fakes;
using VIAPadelClub.Core.Domain.Aggregates.Schedules.Enums;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.ScheduleTests.CreateSchedule;

public class CreateScheduleHandlerTests
{
    private static CreateScheduleCommand ValidCommand()
        => ((Result<CreateScheduleCommand>.Success)
            CreateScheduleCommand.Create("Morning Session", Status.Draft)).Value;

    [Fact]
    public async Task GivenValidCommand_WhenHandlingAsync_ThenReturnsSuccess()
    {
        var repo    = new FakeScheduleRepo();
        var uow     = new FakeUnitOfWork();
        var handler = new CreateScheduleHandler(repo, uow);

        var result = await handler.HandleAsync(ValidCommand());

        Assert.IsType<Result<None>.Success>(result);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandlingAsync_ThenScheduleIsAddedToRepo()
    {
        var repo    = new FakeScheduleRepo();
        var uow     = new FakeUnitOfWork();
        var handler = new CreateScheduleHandler(repo, uow);

        await handler.HandleAsync(ValidCommand());

        Assert.Single(repo.Schedules);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandlingAsync_ThenScheduleIsInDraftStatus()
    {
        var repo    = new FakeScheduleRepo();
        var uow     = new FakeUnitOfWork();
        var handler = new CreateScheduleHandler(repo, uow);

        await handler.HandleAsync(ValidCommand());

        Assert.Equal(Status.Draft, repo.Schedules[0].Status);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandlingAsync_ThenSaveChangesIsCalled()
    {
        var repo    = new FakeScheduleRepo();
        var uow     = new FakeUnitOfWork();
        var handler = new CreateScheduleHandler(repo, uow);

        await handler.HandleAsync(ValidCommand());

        Assert.True(uow.SaveChangesCalled);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandlingAsync_ThenScheduleHasNoCourts()
    {
        var repo    = new FakeScheduleRepo();
        var uow     = new FakeUnitOfWork();
        var handler = new CreateScheduleHandler(repo, uow);

        await handler.HandleAsync(ValidCommand());

        Assert.Empty(repo.Schedules[0].Courts);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandlingAsync_ThenScheduleHasDefaultTimes()
    {
        var repo    = new FakeScheduleRepo();
        var uow     = new FakeUnitOfWork();
        var handler = new CreateScheduleHandler(repo, uow);

        await handler.HandleAsync(ValidCommand());

        var schedule = repo.Schedules[0];
        Assert.Equal(new TimeOnly(15, 0), TimeOnly.FromDateTime(schedule.Times[0].TimeInterval.Start));
        Assert.Equal(new TimeOnly(22, 0), TimeOnly.FromDateTime(schedule.Times[0].TimeInterval.End));
    }

    [Fact]
    public void GivenNullTitle_WhenCreatingCommand_ThenThrowsOrReturnsFailure()
    {
        Assert.Throws<NullReferenceException>(() => CreateScheduleCommand.Create(null!, Status.Draft));
    }
}