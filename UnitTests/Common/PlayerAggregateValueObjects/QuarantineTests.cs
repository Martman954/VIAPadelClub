using VIAPadelClub.Core.Domain.Aggregates.Player.ValueObjects;
using VIAPadelClub.Core.Domain.Common.Values;

namespace UnitTests.Common.PlayerAggregateValueObjects;

public class QuarantineTests
{
    [Fact]
    public void Create_Should_ReturnSuccess_When_ValidInputsProvided()
    {
        // Arrange
        var interval = ValidInterval();
        var email = ValidEmail();

        // Act
        var quarantine = Quarantine.Create(interval, email)
            .AssertSuccess();

        // Assert
        Assert.Equal(interval, quarantine.TimeInterval);
        Assert.Equal(email, quarantine.ViaEmail);
    }

    private static TimeInterval ValidInterval()
        => TimeInterval
            .Create(DateTime.UtcNow, DateTime.UtcNow.AddHours(2))
            .AssertSuccess();

    private static ViaEmail ValidEmail()
        => ViaEmail.CreateEmail("test@test.com")
            .AssertSuccess();
}