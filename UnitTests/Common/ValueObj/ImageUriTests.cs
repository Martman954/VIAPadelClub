using VIAPadelClub.Core.Domain.Aggregates.Players.ValueObjects;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace UnitTests.Common.PlayerAggregateValueObjects;

public class ImageUrlTests
{

    [Theory]
    [InlineData("https://img.com/pic.png")]
    [InlineData("https://img.com/photo.jpg")]
    [InlineData("https://img.com/photo.jpeg")]
    [InlineData("https://img.com/anim.gif")]
    [InlineData("https://img.com/photo.webp")]
    [InlineData("http://img.com/pic.png")]
    public void CreateImageUrl_ValidUrl_ReturnsSuccess(string url)
    {
        var result = ImageUrl.CreateImageUrl(url);

        var imageUrl = result.AssertSuccess();
        Assert.Equal(url, imageUrl.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateImageUrl_NullOrWhitespace_ReturnsFailureWithEmptyMessage(string input)
    {
        var result = ImageUrl.CreateImageUrl(input!);

        var errors = result.AssertFailure();
        Assert.Single(errors);
        Assert.Equal(ErrorType.Validation, errors.First().ErrorType);
        Assert.Equal("Image URL is empty", errors.First().Message);
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("just text")]
    public void CreateImageUrl_InvalidFormat_ReturnsFailureWithFormatMessage(string input)
    {
        var result = ImageUrl.CreateImageUrl(input);

        var errors = result.AssertFailure();
        Assert.Single(errors);
        Assert.Equal(ErrorType.Validation, errors.First().ErrorType);
        Assert.Equal("Invalid URL format", errors.First().Message);
    }

    [Theory]
    [InlineData("ftp://img.com/pic.png")]
    [InlineData("file:///img/pic.png")]
    public void CreateImageUrl_WrongScheme_ReturnsFailureWithSchemeMessage(string input)
    {
        var result = ImageUrl.CreateImageUrl(input);

        var errors = result.AssertFailure();
        Assert.Single(errors);
        Assert.Equal(ErrorType.Validation, errors.First().ErrorType);
        Assert.Equal("URL must be HTTP or HTTPS", errors.First().Message);
    }

    [Theory]
    [InlineData("https://img.com/pic.pdf")]
    [InlineData("https://img.com/pic.mp4")]
    [InlineData("https://img.com/pic")]
    [InlineData("https://img.com/pic.bmp")]
    public void CreateImageUrl_InvalidExtension_ReturnsFailureWithExtensionMessage(string input)
    {
        var result = ImageUrl.CreateImageUrl(input);

        var errors = result.AssertFailure();
        Assert.Single(errors);
        Assert.Equal(ErrorType.Validation, errors.First().ErrorType);
        Assert.Equal("URL must point to an image", errors.First().Message);
    }
}