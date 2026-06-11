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

public class PlayerRegisterAggregateTests
{
    [Fact]
    public void Register_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        var emailResult = ViaEmail.CreateEmail("test@via.dk");
        var nameResult = Name.CreateName("John", "Doe");
        var imageResult = ImageUrl.CreateImageUrl("https://img.com/pic.png");
        
        Assert.True(emailResult is Result<ViaEmail>.Success, "ViaEmail creation failed. Ensure the email formatting follows business rules (like using a @via.dk domain).");
        Assert.True(nameResult is Result<Name>.Success, "Name creation failed.");
        Assert.True(imageResult is Result<ImageUrl>.Success, "ImageUrl creation failed.");

        var email = ((Result<ViaEmail>.Success)emailResult).Value;
        var name = ((Result<Name>.Success)nameResult).Value;
        var image = ((Result<ImageUrl>.Success)imageResult).Value;
        var emailChecker = new EmailAvailableChecker();

        // Act
        var result = Player.Register(email, name, image, emailChecker);

        // Assert
        Assert.True(result is Result<Player>.Success, "Expected player registration to succeed with valid inputs.");
    
        if (result is Result<Player>.Success success)
        {
            Assert.Equal(email, success.Value.Email);
            Assert.Equal(name, success.Value.Name);
        }
    }

    [Fact]
    public void Register_Should_NotBeCalled_When_ValueObjectCreationFails()
    {
        // Arrange & Act
        var emailResult = ViaEmail.CreateEmail("invalid-email-string");

        // Assert
        Assert.True(emailResult is Result<ViaEmail>.Failure, "Value object creation should fail for incorrectly formatted emails.");
    }
}