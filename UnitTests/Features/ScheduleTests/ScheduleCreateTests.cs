using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Aggregates.Schedules.Enums;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.ScheduleTests;

public class ScheduleCreateTests
{
    [Fact]
    public void Create_ReturnsSuccess()
    {
        var result = Schedule.Create();

        Assert.IsType<Result<Schedule>.Success>(result);
    }

    [Fact]
    public void Create_ScheduleIsInDraftStatus()
    {
        var schedule = Schedule.Create().Payload;

        Assert.Equal(Status.Draft, schedule.Status);
    }

    [Fact]
    public void Create_HasOneDefaultTimeSlot()
    {
        var schedule = Schedule.Create().Payload;

        Assert.Single(schedule.Times);
    }

    [Fact]
    public void Create_DefaultTimeSlotIsNotVip()
    {
        var schedule = Schedule.Create().Payload;

        Assert.False(schedule.Times[0].IsVip);
    }

    [Fact]
    public void Create_HasNoCourts()
    {
        var schedule = Schedule.Create().Payload;

        Assert.Empty(schedule.Courts);
    }

    [Fact]
    public void Create_HasUniqueId()
    {
        var a = Schedule.Create().Payload;
        var b = Schedule.Create().Payload;

        Assert.NotEqual(a.Id, b.Id);
    }
}