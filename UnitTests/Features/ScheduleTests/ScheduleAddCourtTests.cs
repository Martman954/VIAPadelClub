using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Schedules;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.ScheduleTests;

file class NoConflictChecker : IScheduleDateConflictChecker
{
    public bool ActiveScheduleExistsOnDate(Guid excludeScheduleId, DateOnly date) => false;
}

public class ScheduleAddCourtTests
{
    [Fact]
    public void AddCourt_ToDraftFutureSchedule_ReturnsSuccess()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));

        var result = schedule.AddCourt(CourtId.Create("S1").Payload);

        Assert.IsType<Result<None>.Success>(result);
    }

    [Fact]
    public void AddCourt_ToDraftFutureSchedule_CourtIsPresent()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        var courtId = CourtId.Create("S1").Payload;

        schedule.AddCourt(courtId);

        Assert.Contains(courtId, schedule.Courts);
    }

    [Fact]
    public void AddCourt_ToActiveSchedule_ReturnsSuccess()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        schedule.AddCourt(CourtId.Create("S1").Payload);
        schedule.Activate(new NoConflictChecker());

        var result = schedule.AddCourt(CourtId.Create("S2").Payload);

        Assert.IsType<Result<None>.Success>(result);
    }

    [Fact]
    public void AddCourt_ToDeletedSchedule_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        typeof(Schedule)
            .GetMethod("Delete", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .Invoke(schedule, new object[] { DateTime.Today });

        var result = schedule.AddCourt(CourtId.Create("S1").Payload);

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("draft or active", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void AddCourt_DuplicateCourt_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        schedule.AddCourt(CourtId.Create("S1").Payload);

        var result = schedule.AddCourt(CourtId.Create("S1").Payload);

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("already", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void AddCourt_ToPastSchedule_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;

        var result = schedule.AddCourt(CourtId.Create("S1").Payload);

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("future", StringComparison.OrdinalIgnoreCase));
    }
}