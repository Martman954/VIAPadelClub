using VIAPadelClub.Core.Domain.Aggregates.Schedule.ValueObjects;
using VIAPadelClub.Core.Domain.Common.Values;

namespace UnitTests.Features.ScheduleTests;
using VIAPadelClub.Core.Domain.Aggregates.Schedule;
using VIAPadelClub.Core.Tools.OperationResult.Results;

/// <summary>
/// Testing the Schedule creation
/// </summary>
public class ScheduleCreateTests
{
    [Fact]
    public void Create_ShouldReturnFailure_When_IntervalsAreNull()
    {
        // Arrange
        var date = DateTime.Today;
        List<ScheduleTimeInterval>? intervals = null;

        // Act
        var result = Schedule.Create(date, intervals!);

        // Assert
        var failure = Assert.IsType<Result<Schedule>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message == "Schedule must contain at least one time interval");
    }
    
    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var tomorrow = DateTime.Today.AddDays(1);
        var start = tomorrow.AddHours(8); 
        var end = tomorrow.AddHours(10);
        
        var validIntervals = new List<ScheduleTimeInterval>
        {
            new ScheduleTimeInterval(TimeInterval.Create(start, end), true) 
        };

        // Act
        var result = Schedule.Create(tomorrow, validIntervals);

        // Assert
        if (result is Result<Schedule>.Success success)
        {
            Assert.Equal(tomorrow, success.Value.Date);
            Assert.Equal(validIntervals.Count, success.Value.ActiveTime.Count);
        }
        else
        {
            Assert.Fail("Expected a successful Schedule creation but received a failure.");
        }
    }
    
    [Fact]
    public void Create_Should_ThrowException_When_TimeIntervalIsInvalid()
    {
        // Arrange
        var date = DateTime.Today.AddDays(1);
        var start = date.AddHours(10);
        var end = date.AddHours(8); // Invalid: End before start

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new TimeInterval(start, end)
        );
        Assert.Equal("End must be after start", exception.Message);
    }
}