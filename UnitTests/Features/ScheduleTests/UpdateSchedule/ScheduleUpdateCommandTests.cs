using VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.ScheduleTests.UpdateSchedule;

public class ScheduleUpdateCommandTests
{
    private static readonly Guid ValidScheduleId = Guid.NewGuid();
    private static readonly DateOnly ValidDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
    private static readonly TimeOnly ValidStart = new(15, 0);
    private static readonly TimeOnly ValidEnd = new(16, 0);

    [Fact]
    public void GivenValidInputs_WhenCreatingCommand_ThenReturnsSuccess()
    {
        var result = UpdateScheduleDateTimeCommand.Create(
            ValidScheduleId.ToString(),
            ValidDate,
            ValidStart,
            ValidEnd);

        Assert.IsType<Result<UpdateScheduleDateTimeCommand>.Success>(result);
    }

    [Fact]
    public void GivenValidInputs_WhenCreatingCommand_ThenCommandHasCorrectScheduleId()
    {
        var result = UpdateScheduleDateTimeCommand.Create(
            ValidScheduleId.ToString(),
            ValidDate,
            ValidStart,
            ValidEnd);

        var success = Assert.IsType<Result<UpdateScheduleDateTimeCommand>.Success>(result);
        Assert.Equal(ValidScheduleId, success.Value.ScheduleId);
    }

    [Fact]
    public void GivenValidInputs_WhenCreatingCommand_ThenCommandHasCorrectTimeInterval()
    {
        var result = UpdateScheduleDateTimeCommand.Create(
            ValidScheduleId.ToString(),
            ValidDate,
            ValidStart,
            ValidEnd);

        var success = Assert.IsType<Result<UpdateScheduleDateTimeCommand>.Success>(result);
        Assert.Equal(new DateTime(ValidDate, ValidStart), success.Value.ScheduleTimeInterval.TimeInterval.Start);
        Assert.Equal(new DateTime(ValidDate, ValidEnd), success.Value.ScheduleTimeInterval.TimeInterval.End);
    }
}