using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Aggregates.Schedules.Enums;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Schedules;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.ScheduleTests.ActiveSchedule;

file class NoConflictChecker : IScheduleDateConflictChecker
{
    public bool ActiveScheduleExistsOnDate(ScheduleId excludeScheduleId, DateOnly date) => false;
}

file class ConflictChecker : IScheduleDateConflictChecker
{
    public bool ActiveScheduleExistsOnDate(ScheduleId excludeScheduleId, DateOnly date) => true;
}

public class ScheduleActivateAggregateTests
{
    [Fact]
    public void Activate_DeletedSchedule_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        typeof(Schedule)
            .GetMethod("Delete", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .Invoke(schedule, [DateTime.Today]);

        var result = schedule.Activate(new NoConflictChecker());

        Assert.IsType<Result<None>.Failure>(result);
    }

    [Fact]
    public void Activate_AlreadyActiveSchedule_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        schedule.AddCourt(CourtId.Create("S1").Payload);
        schedule.Activate(new NoConflictChecker());

        var result = schedule.Activate(new NoConflictChecker());

        Assert.IsType<Result<None>.Failure>(result);
    }

    [Fact]
    public void Activate_WithNoCourts_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;

        var result = schedule.Activate(new NoConflictChecker());

        Assert.IsType<Result<None>.Failure>(result);
    }

    [Fact]
    public void Activate_WithNoCourts_ErrorMentionsCourt()
    {
        var schedule = Schedule.Create().Payload;

        var result = schedule.Activate(new NoConflictChecker());

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("court", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Activate_WhenDateConflictExists_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        schedule.AddCourt(CourtId.Create("S1").Payload);

        var result = schedule.Activate(new ConflictChecker());

        Assert.IsType<Result<None>.Failure>(result);
    }

    [Fact]
    public void Activate_WhenDateConflictExists_ErrorMentionsDate()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        schedule.AddCourt(CourtId.Create("S1").Payload);

        var result = schedule.Activate(new ConflictChecker());

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("date", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Activate_ValidFutureScheduleWithCourt_ReturnsSuccess()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        schedule.AddCourt(CourtId.Create("S1").Payload);

        var result = schedule.Activate(new NoConflictChecker());

        Assert.IsType<Result<None>.Success>(result);
    }

    [Fact]
    public void Activate_ValidFutureScheduleWithCourt_StatusBecomesActive()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        schedule.AddCourt(CourtId.Create("S1").Payload);
        schedule.Activate(new NoConflictChecker());

        Assert.Equal(Status.Active, schedule.Status);
    }
}