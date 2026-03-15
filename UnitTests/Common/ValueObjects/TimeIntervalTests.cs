using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace UnitTests.Common.Bases;

public class TimeIntervalTests
{
    [Fact]
    public void Create_Should_ReturnSuccess_When_StartIsBeforeEnd()
    {
        // Arrange
        var start = DateTime.UtcNow;
        var end = start.AddHours(2);

        // Act
        var interval = TimeInterval.Create(start, end)
            .AssertSuccess();

        // Assert
        Assert.Equal(start, interval.Start);
        Assert.Equal(end, interval.End);
        Assert.Equal(TimeSpan.FromHours(2), interval.Duration);
    }

    [Fact]
    public void Create_Should_ReturnSuccess_When_StartEqualsEnd()
    {
        var now = DateTime.UtcNow;

        var interval = TimeInterval.Create(now, now)
            .AssertSuccess();

        Assert.Equal(TimeSpan.Zero, interval.Duration);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_StartIsAfterEnd()
    {
        // Arrange
        var start = DateTime.UtcNow;
        var end = start.AddHours(-1);

        // Act
        var result = TimeInterval.Create(start, end);

        // Assert
        var errors = result.AssertFailure();

        Assert.Single(errors);
        Assert.Equal(ErrorType.Validation, errors.First().ErrorType);
        Assert.Equal("Time interval not in correct format",
            errors.First().Message);
    }
}