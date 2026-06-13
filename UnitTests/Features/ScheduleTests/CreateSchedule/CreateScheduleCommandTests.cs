using Features.CommandDispatch.ScheduleCommands;
using VIAPadelClub.Core.Domain.Aggregates.Schedules.Enums;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.ScheduleTests.CreateSchedule;

public class CreateScheduleCommandTests
{
    private const string ValidTitle = "Morning Session";
    private const Status ValidStatus = Status.Draft;

    [Fact]
    public void GivenValidInputs_WhenCreatingCommand_ThenReturnsSuccess()
    {
        var result = CreateScheduleCommand.Create(ValidTitle, ValidStatus);

        Assert.IsType<Result<CreateScheduleCommand>.Success>(result);
    }

    [Fact]
    public void GivenValidInputs_WhenCreatingCommand_ThenCommandHasCorrectTitle()
    {
        var result = CreateScheduleCommand.Create(ValidTitle, ValidStatus);

        var success = Assert.IsType<Result<CreateScheduleCommand>.Success>(result);
        Assert.Equal(ValidTitle, success.Value.Title);
    }

    [Fact]
    public void GivenValidInputs_WhenCreatingCommand_ThenCommandHasCorrectStatus()
    {
        var result = CreateScheduleCommand.Create(ValidTitle, ValidStatus);

        var success = Assert.IsType<Result<CreateScheduleCommand>.Success>(result);
        Assert.Equal(ValidStatus, success.Value.Status);
    }

    [Fact]
    public void GivenEmptyTitle_WhenCreatingCommand_ThenReturnsFailure()
    {
        var result = CreateScheduleCommand.Create("", ValidStatus);

        Assert.IsType<Result<CreateScheduleCommand>.Failure>(result);
    }

    [Fact]
    public void GivenEmptyTitle_WhenCreatingCommand_ThenErrorMentionsTitle()
    {
        var result = CreateScheduleCommand.Create("", ValidStatus);

        var failure = Assert.IsType<Result<CreateScheduleCommand>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("title", StringComparison.OrdinalIgnoreCase));
    }
}