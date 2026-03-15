
using VIAPadelClub.Core.Domain.Aggregates.Player;
using VIAPadelClub.Core.Domain.Aggregates.Player.ValueObjects;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.PlayerTests.Register;

public class PlayerRegisterTests
{
    [Fact]
    public void Register_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        var email = Assert
            .IsType<Result<ViaEmail>.Success>(
                ViaEmail.CreateEmail("test@test.com"))
            .Value;

        var name = Assert
            .IsType<Result<Name>.Success>(
                Name.CreateName("John", "Doe"))
            .Value;

        var image = Assert
            .IsType<Result<ImageUrl>.Success>(
                ImageUrl.CreateImageUrl("https://img.com/pic.png"))
            .Value;

        // Act
        Result<Player> result = Player.Register(email, name, image);

        // Assert
        var success = Assert.IsType<Result<Player>.Success>(result);

        Assert.Equal(email, success.Value.Email);
    }
    [Fact]
    public void Register_Should_NotBeCalled_When_ValueObjectCreationFails()
    {
        // Arrange
        var emailResult = ViaEmail.CreateEmail("invalid");
        var name = Name.CreateName("John", "Doe").AssertSuccess();
        var image = ImageUrl.CreateImageUrl("https://img.com/pic.png").AssertSuccess();

        // Act + Assert
        Assert.IsType<Result<ViaEmail>.Failure>(emailResult);
    }
}