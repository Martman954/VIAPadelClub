using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Schedules;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.ScheduleTests;

file class NoConflictChecker : IScheduleDateConflictChecker
{
    public bool ActiveScheduleExistsOnDate(Guid excludeScheduleId, DateOnly date) => false;
}

public class ScheduleUpdateDateAggregateTests
{
    [Fact]
    public void UpdateDate_ToFutureDate_ReturnsSuccess()
    {
        var schedule = Schedule.Create().Payload;

        var result = schedule.UpdateDate(DateTime.Today.AddDays(1));
        
        Assert.True(result is Result<None>.Success, "Expected a successful result.");
    }

    [Fact]
    public void UpdateDate_ToDateInPast_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;

        var result = schedule.UpdateDate(DateTime.Today.AddDays(-1));
        
        if (result is Result<None>.Failure failure)
        {
            Assert.Contains(failure.Errors, e => e.Message.Contains("past", StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            Assert.Fail($"Expected a failure result but received: {result.GetType().Name}");
        }
    }

    [Fact]
    public void UpdateDate_OnActiveSchedule_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        schedule.AddCourt(CourtId.Create("S1").Payload);
        schedule.Activate(new NoConflictChecker());

        var result = schedule.UpdateDate(DateTime.Today.AddDays(2));

        if (result is Result<None>.Failure failure)
        {
            Assert.Contains(failure.Errors, e => e.Message.Contains("Draft", StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            Assert.Fail($"Expected a failure result but received: {result.GetType().Name}");
        }
    }

    [Fact]
    public void UpdateDate_OnDeletedSchedule_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        
        typeof(Schedule)
            .GetMethod("Delete", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .Invoke(schedule, [DateTime.Today]);

        var result = schedule.UpdateDate(DateTime.Today.AddDays(2));

        Assert.True(result is Result<None>.Failure, "Expected a failure result on a deleted schedule.");
    }
}