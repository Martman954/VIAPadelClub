using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Aggregates.Schedules.Enums;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.ScheduleTests;

public class ScheduleDeleteTests
{
    private static Result<None> CallDelete(Schedule schedule, DateTime currentTime)
        => (Result<None>)typeof(Schedule)
            .GetMethod("Delete", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .Invoke(schedule, [currentTime])!;

    [Fact]
    public void Delete_AlreadyDeleted_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        CallDelete(schedule, DateTime.Today);

        var result = CallDelete(schedule, DateTime.Today);

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("already deleted", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Delete_PastSchedule_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;

        var result = CallDelete(schedule, DateTime.Today.AddDays(1));

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("past", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Delete_OnSameDayAsSchedule_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;

        var result = CallDelete(schedule, DateTime.Today);

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("same date", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Delete_FutureSchedule_ReturnsSuccess()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));

        var result = CallDelete(schedule, DateTime.Today);

        Assert.IsType<Result<None>.Success>(result);
    }

    [Fact]
    public void Delete_FutureSchedule_StatusBecomesDeleted()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        CallDelete(schedule, DateTime.Today);

        Assert.Equal(Status.Deleted, schedule.Status);
    }

    [Fact]
    public void Delete_FutureSchedule_CourtsAreCleared()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        schedule.AddCourt(CourtId.Create("S1").Payload);
        CallDelete(schedule, DateTime.Today);

        Assert.Empty(schedule.Courts);
    }
}