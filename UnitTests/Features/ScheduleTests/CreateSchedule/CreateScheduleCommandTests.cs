using VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;

namespace UnitTests.Features.ScheduleTests.CreateSchedule;

public class CreateScheduleCommandTests
{
    [Fact]
    public void Constructor_WhenCalled_CreatesCommandInstance()
    {
        var command = new CreateScheduleCommand();

        Assert.NotNull(command);
    }

    [Fact]
    public void Constructor_WhenCalled_CreatesDistinctInstances()
    {
        var first = new CreateScheduleCommand();
        var second = new CreateScheduleCommand();

        Assert.NotSame(first, second);
    }
}