using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Schedules;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.ScheduleTests.UpdateSchedule;

file class NoConflictChecker : IScheduleDateConflictChecker
{
    public bool ActiveScheduleExistsOnDate(Guid excludeScheduleId, DateOnly date) => false;
}

public class ScheduleUpdateTimeAggregateTests
{
    private static TimeInterval TargetDayInterval(DateTime baseDate, int startHour = 10, int endHour = 20)
    {
        return TimeInterval.Create(baseDate.Date.AddHours(startHour), baseDate.Date.AddHours(endHour)).Payload;
    }

    [Fact]
    public void UpdateTimes_OnDraftSchedule_ReturnsSuccess()
    {
        var schedule = Schedule.Create().Payload;
        var targetDate = DateTime.Today.AddDays(1);
        schedule.UpdateDate(targetDate);

        var result = schedule.UpdateTimes(TargetDayInterval(targetDate));

        Assert.True(result is Result<None>.Success, "Expected a successful result mapping.");
    }

    [Fact]
    public void UpdateTimes_OnActiveSchedule_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;
        var targetDate = DateTime.Today.AddDays(1);
        schedule.UpdateDate(targetDate);
        schedule.AddCourt(CourtId.Create("S1").Payload);
        schedule.Activate(new NoConflictChecker());

        var result = schedule.UpdateTimes(TargetDayInterval(targetDate));

        if (result is Result<None>.Failure failure)
        {
            Assert.Contains(failure.Errors, e => e.Message.Contains("Draft", StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            Assert.Fail($"Expected Draft validation failure, but received: {result.GetType().Name}");
        }
    }

    [Fact]
    public void UpdateTimes_OnDeletedSchedule_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;
        var targetDate = DateTime.Today.AddDays(1);
        schedule.UpdateDate(targetDate);
        
        typeof(Schedule)
            .GetMethod("Delete", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .Invoke(schedule, [DateTime.Today]);

        var result = schedule.UpdateTimes(TargetDayInterval(targetDate));

        Assert.True(result is Result<None>.Failure, "Expected a failure state response on a deleted entity structure.");
    }
}