using VIAPadelClub.Core.Domain.Aggregates.Schedule;
using VIAPadelClub.Core.Domain.Aggregates.Schedule.ValueObjects;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.ScheduleTests;

public class AddCourtsToScheduleTests
{
     private Schedules CreateValidSchedule(DateTime? date = null)
    {
        var scheduleDate = date ?? DateTime.Today.AddDays(1);
        var timeIntervalResult = TimeInterval.Create(scheduleDate.AddHours(15), scheduleDate.AddHours(22));
        var timeInterval = Assert.IsType<Result<TimeInterval>.Success>(timeIntervalResult).Value;
    
        var scheduleIntervalResult = ScheduleTimeInterval.Create(timeInterval, false);
        var scheduleInterval = Assert.IsType<Result<ScheduleTimeInterval>.Success>(scheduleIntervalResult).Value;
    
        var intervals = new List<ScheduleTimeInterval> { scheduleInterval };
        var result = Schedules.Create(scheduleInterval, intervals);
        return Assert.IsType<Result<Schedules>.Success>(result).Value;
    }
    private CourtId CreateCourtId(string name)
    {
        var result = CourtId.Create(name);
        return Assert.IsType<Result<CourtId>.Success>(result).Value;
    }

    // S1 - General success
    [Fact]
    public void AddCourt_ShouldSucceed_WhenScheduleIsActiveAndCourtIsValid()
    {
        var schedule = CreateValidSchedule();
        schedule.Activate();
        var courtId = CreateCourtId("S1");

        var result = schedule.AddCourt(courtId);

        Assert.IsType<Result<None>.Success>(result);
        Assert.Contains(schedule.Courts, c => c.ToString() == "S1");
    }

    // S2 - First court added
    [Fact]
    public void AddCourt_ShouldSucceed_WhenNoCourtsExist()
    {
        var schedule = CreateValidSchedule();
        var courtId = CreateCourtId("D1");

        var result = schedule.AddCourt(courtId);

        Assert.IsType<Result<None>.Success>(result);
        Assert.Single(schedule.Courts);
    }

    // S3 - Add with existing courts of different names
    [Theory]
    [InlineData("D1", "D2")]
    [InlineData("D2", "S1")]
    public void AddCourt_ShouldSucceed_WhenCourtsHaveDifferentNames(string existing, string added)
    {
        var schedule = CreateValidSchedule();
        schedule.AddCourt(CreateCourtId(existing));

        var result = schedule.AddCourt(CreateCourtId(added));

        Assert.IsType<Result<None>.Success>(result);
        Assert.Equal(2, schedule.Courts.Count);
    }

    // S1 - Name is capitalized
    [Fact]
    public void AddCourt_ShouldCapitalizeName_WhenLowercaseProvided()
    {
        var schedule = CreateValidSchedule();
        var courtId = CreateCourtId("s1");

        schedule.AddCourt(courtId);

        Assert.Contains(schedule.Courts, c => c.ToString() == "S1");
    }
    
    // F7 - Court already exists
    [Fact]
    public void AddCourt_ShouldFail_WhenCourtAlreadyExists()
    {
        var schedule = CreateValidSchedule();
        var courtId = CreateCourtId("S1");
        schedule.AddCourt(courtId);

        var result = schedule.AddCourt(CreateCourtId("S1"));

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message == "This court is already added to the schedule.");
    }

    // F2 - Invalid starting letter
    [Fact]
    public void AddCourt_ShouldFail_WhenCourtNameStartsWithInvalidLetter()
    {
        var result = CourtId.Create("A1");

        var failure = Assert.IsType<Result<CourtId>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message == "Court name must start with 'S' or 'D'.");
    }

    // F4 - Invalid ending number
    [Fact]
    public void AddCourt_ShouldFail_WhenCourtNameEndsWithInvalidNumber()
    {
        var result = CourtId.Create("S11");

        var failure = Assert.IsType<Result<CourtId>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message == "Court number must be between 1 and 10.");
    }

    // F5 - Invalid name length
    [Theory]
    [InlineData("S")]
    [InlineData("S123")]
    public void AddCourt_ShouldFail_WhenCourtNameIsInvalidLength(string name)
    {
        var result = CourtId.Create(name);

        var failure = Assert.IsType<Result<CourtId>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message == "Court name must be 2 or 3 characters long.");
    }

}