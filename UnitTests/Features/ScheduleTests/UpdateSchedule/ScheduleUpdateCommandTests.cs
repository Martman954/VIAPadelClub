using Features.CommandDispatch.ScheduleCommands;
using VIAPadelClub.Core.Domain.Aggregates.Schedules.ValueObjects;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.ScheduleTests.UpdateSchedule;

public class UpdateScheduleCommandTests
{
    private static readonly Guid ValidScheduleId = Guid.NewGuid();

    private static ScheduleTimeInterval CreateValidInterval()
    {
        var start = DateTime.Today.AddDays(1).AddHours(15);
        var end = start.AddHours(1);
        var interval = TimeInterval.Create(start, end).Payload;
        return ((Result<ScheduleTimeInterval>.Success)ScheduleTimeInterval.Create(interval, false)).Value;
    }

    [Fact]
    public void GivenValidInputs_WhenCreatingCommand_ThenReturnsSuccess()
    {
        var result = UpdateScheduleCommand.Create(ValidScheduleId, CreateValidInterval());

        Assert.IsType<Result<UpdateScheduleCommand>.Success>(result);
    }

    [Fact]
    public void GivenValidInputs_WhenCreatingCommand_ThenCommandHasCorrectScheduleId()
    {
        var result = UpdateScheduleCommand.Create(ValidScheduleId, CreateValidInterval());

        var success = Assert.IsType<Result<UpdateScheduleCommand>.Success>(result);
        Assert.Equal(ValidScheduleId, success.Value.ScheduleId);
    }

    [Fact]
    public void GivenValidInputs_WhenCreatingCommand_ThenCommandHasCorrectTimeInterval()
    {
        var interval = CreateValidInterval();

        var result = UpdateScheduleCommand.Create(ValidScheduleId, interval);

        var success = Assert.IsType<Result<UpdateScheduleCommand>.Success>(result);
        Assert.Equal(interval, success.Value.ScheduleTimeInterval);
    }
}