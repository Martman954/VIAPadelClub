using System.Reflection;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.ValueObjects;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Players;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.PlayerTests;
file class EmailAvailableChecker : IEmailInUseChecker
{
    public bool IsEmailInUse(ViaEmail email) => false;
}

public class PlayerStatusTests
{

    [Fact]
    public void Blacklist_OnActivePlayer_ClearsVipAndQuarantineStatuses()
    {
        // Arrange
        var player = CreateTestPlayer();
        var today = DateTime.Today;

        player.ElevateToVip(today);
        InvokeQuarantinePlayer(player, today);

        // Act
        var result = InvokeBlacklist(player);

        // Assert
        Assert.True(result is Result<None>.Success);
        Assert.True(player.isBlackListed);
        Assert.Null(player.VipStatus);
        Assert.False(player.IsQuarantined(today), "Active quarantine must be wiped upon blacklisting.");
    }

    [Fact]
    public void LiftBlacklist_WhenPlayerIsBlacklisted_SetsBlacklistedFalse()
    {
        // Arrange
        var player = CreateTestPlayer();
        InvokeBlacklist(player);

        // Act
        var result = player.LiftBlacklist();

        // Assert
        Assert.True(result is Result<None>.Success);
        Assert.False(player.isBlackListed);
    }

    [Fact]
    public void LiftBlacklist_WhenPlayerIsNormal_ReturnsValidationFailure()
    {
        // Arrange
        var player = CreateTestPlayer();

        // Act
        var result = player.LiftBlacklist();

        // Assert
        Assert.True(result is Result<None>.Failure);
        if (result is Result<None>.Failure failure)
        {
            Assert.Contains(failure.Errors, e => e.Message.Contains("not blacklisted", StringComparison.OrdinalIgnoreCase));
        }
    }

    [Fact]
    public void ElevateToVip_WhenPlayerIsNormal_CreatesVipStatusSuccessfully()
    {
        // Arrange
        var player = CreateTestPlayer();
        var today = DateTime.Today;

        // Act
        var result = player.ElevateToVip(today);

        // Assert
        Assert.True(result is Result<None>.Success);
        Assert.NotNull(player.VipStatus);
    }

    [Fact]
    public void ElevateToVip_WhenPlayerIsBlacklisted_ReturnsValidationFailure()
    {
        // Arrange
        var player = CreateTestPlayer();
        InvokeBlacklist(player);

        // Act
        var result = player.ElevateToVip(DateTime.Today);

        // Assert
        Assert.True(result is Result<None>.Failure);
        if (result is Result<None>.Failure failure)
        {
            Assert.Contains(failure.Errors, e => e.Message.Contains("Blacklisted", StringComparison.OrdinalIgnoreCase));
        }
    }

    [Fact]
    public void ElevateToVip_WhenPlayerIsQuarantined_ReturnsValidationFailure()
    {
        // Arrange
        var player = CreateTestPlayer();
        var today = DateTime.Today;
        InvokeQuarantinePlayer(player, today);

        // Act
        var result = player.ElevateToVip(today);

        // Assert
        Assert.True(result is Result<None>.Failure);
        if (result is Result<None>.Failure failure)
        {
            Assert.Contains(failure.Errors, e => e.Message.Contains("Quarantined", StringComparison.OrdinalIgnoreCase));
        }
    }

    [Fact]
    public void ElevateToVip_WhenPlayerIsAlreadyVip_TriggersExtensionBranch()
    {
        // Arrange
        var player = CreateTestPlayer();
        var today = DateTime.Today;

        player.ElevateToVip(today);

        // Act
        var result = player.ElevateToVip(today);

        // Assert
        Assert.True(result is Result<None>.Success);
        Assert.NotNull(player.VipStatus);
    }

    private static Player CreateTestPlayer()
    {
        var email = ((Result<ViaEmail>.Success)ViaEmail.CreateEmail("123456@via.dk")).Value;
        var name = ((Result<Name>.Success)Name.CreateName("Alex", "Andersen")).Value;
        var image = ((Result<ImageUrl>.Success)ImageUrl.CreateImageUrl("https://via.dk/pic.png")).Value;
        
        var registerResult = Player.Register(email, name, image, new EmailAvailableChecker());
        
        if (registerResult is Result<Player>.Success success)
        {
            return success.Value;
        }

        throw new InvalidOperationException("Failed to construct test player baseline.");
    }

    private static object InvokeBlacklist(Player player)
    {
        var method = typeof(Player).GetMethod("Blacklist", 
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        return method!.Invoke(player, null)!;
    }

    private static void InvokeQuarantinePlayer(Player player, DateTime date)
    {
        var method = typeof(Player).GetMethod("QuarantinePlayer", 
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        method!.Invoke(player, [date]);
    }
}