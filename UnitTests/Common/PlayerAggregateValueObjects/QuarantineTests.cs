using Xunit;
using VIAPadelClub.Core.Domain.Aggregates.Players.ValueObjects;

namespace UnitTests.Common.PlayerAggregateValueObjects;

public class QuarantineTests
{
    [Fact]
    public void Create_Should_ReturnActiveQuarantine_When_ValidCurrentDateProvided()
    {
        // Arrange
        var baseDate = DateTime.Today;

        // Act - Calling the method directly without a Result wrapper
        Quarantine quarantine = Quarantine.Create(baseDate);

        // Assert
        Assert.NotNull(quarantine);
        Assert.True(quarantine.IsActive(baseDate), "Quarantine should be active on the day it was created.");
    }

    [Fact]
    public void IsActive_Should_ReturnFalse_When_QuarantinePeriodHasExpired()
    {
        // Arrange
        var baseDate = DateTime.Today;
        Quarantine quarantine = Quarantine.Create(baseDate);

        // Act
        var futureDate = baseDate.AddDays(10);
        var isActiveInFuture = quarantine.IsActive(futureDate);

        // Assert
        Assert.False(isActiveInFuture, "Quarantine should automatically expire after its penalty window passes.");
    }
}