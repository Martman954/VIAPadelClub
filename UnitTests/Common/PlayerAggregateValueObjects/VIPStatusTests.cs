using VIAPadelClub.Core.Domain.Common.Bases;
using VIAPadelClub.Core.Domain.Common.Values.Player;

namespace UnitTests.Common.PlayerAggregateValueObjects;

public class VipStatusTests
{
    [Fact]
    public void Create_Should_ReturnSuccess_When_TimeIntervalProvided()
    {
        // Arrange
        var interval = ValidInterval();

        // Act
        var vipStatus = VipStatus.Create(interval)
            .AssertSuccess();

        // Assert
        Assert.Equal(interval, vipStatus.TimeInterval);
    }

    private static TimeInterval ValidInterval()
    {
        return TimeInterval
            .Create(DateTime.UtcNow, DateTime.UtcNow.AddHours(1))
            .AssertSuccess();
    }
}