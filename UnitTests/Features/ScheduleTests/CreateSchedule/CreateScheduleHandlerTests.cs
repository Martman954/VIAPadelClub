using VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;
using VIAPadelClub.Core.Application.Features.Schedules;
using UnitTests.Fakes;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.ScheduleTests.CreateSchedule;

public class CreateScheduleHandlerTests
{
    private static CreateScheduleCommand ValidCommand()
        => new CreateScheduleCommand();

    [Fact]
    public async Task GivenValidCommand_WhenHandlingAsync_ThenReturnsSuccess()
    {
        var repo    = new FakeScheduleRepo();
        var handler = new CreateScheduleHandler(repo);

        var result = await handler.HandleAsync(ValidCommand());

        Assert.IsType<Result<None>.Success>(result);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandlingAsync_ThenScheduleIsAddedToRepo()
    {
        var repo    = new FakeScheduleRepo();
        var handler = new CreateScheduleHandler(repo);

        await handler.HandleAsync(ValidCommand());

        Assert.Single(repo.Schedules);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandlingAsync_ThenScheduleIsInDraftStatus()
    {
        var repo    = new FakeScheduleRepo();
        var handler = new CreateScheduleHandler(repo);

        await handler.HandleAsync(ValidCommand());

        Assert.Equal(VIAPadelClub.Core.Domain.Aggregates.Schedules.Enums.Status.Draft, repo.Schedules[0].Status);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandlingAsync_ThenScheduleHasNoCourts()
    {
        var repo    = new FakeScheduleRepo();
        var handler = new CreateScheduleHandler(repo);

        await handler.HandleAsync(ValidCommand());

        Assert.Empty(repo.Schedules[0].Courts);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandlingAsync_ThenScheduleHasDefaultTimes()
    {
        var repo    = new FakeScheduleRepo();
        var handler = new CreateScheduleHandler(repo);

        await handler.HandleAsync(ValidCommand());

        var schedule = repo.Schedules[0];
        Assert.Equal(new TimeOnly(15, 0), TimeOnly.FromDateTime(schedule.Times[0].TimeInterval.Start));
        Assert.Equal(new TimeOnly(22, 0), TimeOnly.FromDateTime(schedule.Times[0].TimeInterval.End));
    }

}