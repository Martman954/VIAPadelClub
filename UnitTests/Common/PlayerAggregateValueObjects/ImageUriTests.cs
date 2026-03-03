using VIAPadelClub.Core.Domain.Common.Values.Player;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace UnitTests.Common.PlayerAggregateValueObjects;

public class ImageUrlTests
{
    [Fact]
    public void CreateImageUrl_Should_ReturnSuccess_When_UrlIsValid()
    {
        // Act
        var imageUrl = ImageUrl
            .CreateImageUrl("https://img.com/pic.png")
            .AssertSuccess();

        // Assert
        Assert.Equal("https://img.com/pic.png", imageUrl.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateImageUrl_Should_ReturnFailure_When_UrlIsInvalid(string input)
    {
        // Act
        var result = ImageUrl.CreateImageUrl(input!);

        // Assert
        var errors = result.AssertFailure();

        Assert.Single(errors);
        Assert.Equal(ErrorType.Validation, errors.First().ErrorType);
        Assert.Equal("Image URL not in correct format",
            errors.First().Message);
    }
}