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

public class PlayerQuarantineAggregateTests
{
    private static Player CreateValidTestPlayer()
    {
        var emailResult = ViaEmail.CreateEmail("123456@via.dk");
        var nameResult = Name.CreateName("Alex", "Andersen");
        var imageResult = ImageUrl.CreateImageUrl("https://via.dk/pic.png");

        if (emailResult is Result<ViaEmail>.Failure emailFailure)
        {
            var errors = string.Join(", ", emailFailure.Errors.Select(e => e.Message));
            throw new Xunit.Sdk.XunitException($"ViaEmail validation blocked test setup! Reason(s): {errors}");
        }
        if (nameResult is Result<Name>.Failure nameFailure)
        {
            throw new Xunit.Sdk.XunitException($"Name validation blocked test setup! Reason: {nameFailure.Errors.FirstOrDefault()?.Message}");
        }
        if (imageResult is Result<ImageUrl>.Failure imageFailure)
        {
            throw new Xunit.Sdk.XunitException($"ImageUrl validation blocked test setup! Reason: {imageFailure.Errors.FirstOrDefault()?.Message}");
        }

        var email = ((Result<ViaEmail>.Success)emailResult).Value;
        var name = ((Result<Name>.Success)nameResult).Value;
        var image = ((Result<ImageUrl>.Success)imageResult).Value;
        
        var registerResult = Player.Register(email, name, image, new EmailAvailableChecker());
        
        if (registerResult is Result<Player>.Success success)
        {
            return success.Value; 
        }
        
        throw new InvalidOperationException("Failed to construct test player baseline.");
    }

    private static object InvokeQuarantinePlayer(Player player, DateTime date)
    {
        var method = typeof(Player).GetMethod("QuarantinePlayer", 
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            
        if (method == null)
        {
            throw new MissingMethodException(nameof(Player), "QuarantinePlayer");
        }

        return method.Invoke(player, [date])!;
    }

    [Fact]
    public void QuarantinePlayer_WhenNotAlreadyQuarantined_CreatesNewQuarantine()
    {
        // Arrange
        var player = CreateValidTestPlayer();
        var today = DateTime.Today;

        // Act
        var result = InvokeQuarantinePlayer(player, today);

        // Assert
        Assert.True(result is Result<None>.Success, "Expected quarantine assignment to succeed.");
        Assert.True(player.IsQuarantined(today), "Player should report as actively quarantined today.");
    }

    [Fact]
    public void IsQuarantined_WhenCheckedAfterQuarantineExpires_ReturnsFalse()
    {
        // Arrange
        var player = CreateValidTestPlayer();
        var today = DateTime.Today;
        
        InvokeQuarantinePlayer(player, today);

        // Act
        var futureDate = today.AddDays(14);
        var isQuarantinedInFuture = player.IsQuarantined(futureDate);

        // Assert
        Assert.False(isQuarantinedInFuture, "Player should no longer be quarantined after the period expires.");
    }

    [Fact]
    public void QuarantinePlayer_WhenAlreadyQuarantined_InvokesExtensionLogic()
    {
        // Arrange
        var player = CreateValidTestPlayer();
        var today = DateTime.Today;

        InvokeQuarantinePlayer(player, today);

        // Act
        var result = InvokeQuarantinePlayer(player, today);

        // Assert
        Assert.True(result is Result<None>.Success);
        Assert.True(player.IsQuarantined(today), "Player should still be quarantined.");
    }
}