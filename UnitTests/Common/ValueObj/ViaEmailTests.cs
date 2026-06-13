using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace UnitTests.Common.PlayerAggregateValueObjects;

public class ViaEmailTests
{

    [Theory]
    [InlineData("123456@via.dk")]   // 6-digit student number
    [InlineData("abc@via.dk")]      // 3-letter staff
    [InlineData("abcd@via.dk")]     // 4-letter staff
    [InlineData("ABC@via.dk")]      // uppercase normalized
    public void CreateEmail_ValidViaEmail_ReturnsSuccess(string email)
    {
        var result = ViaEmail.CreateEmail(email);

        var success = Assert.IsType<Result<ViaEmail>.Success>(result);
        Assert.Equal(email, success.Value.Value);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateEmail_NullOrEmpty_ReturnsFailureWithEmptyMessage(string? input)
    {
        if (input != null)
        {
            var result = ViaEmail.CreateEmail(input);

            var failure = Assert.IsType<Result<ViaEmail>.Failure>(result);
            Assert.Single(failure.Errors);
            Assert.Equal(ErrorType.Validation, failure.Errors.First().ErrorType);
            Assert.Equal("Email is empty", failure.Errors.First().Message);
        }
    }

    [Theory]
    [InlineData("test@test.com")]       // wrong domain
    [InlineData("invalidemail.com")]    // no @ symbol
    [InlineData("12345@via.dk")]        // 5 digits, not 6
    [InlineData("abcde@via.dk")]        // 5 letters, not 3-4
    [InlineData("ab@via.dk")]           // 2 letters, too short
    [InlineData("123456@gmail.com")]    // wrong domain
    public void CreateEmail_InvalidViaFormat_ReturnsFailureWithFormatMessage(string input)
    {
        var result = ViaEmail.CreateEmail(input);

        var failure = Assert.IsType<Result<ViaEmail>.Failure>(result);
        Assert.Single(failure.Errors);
        Assert.Equal(ErrorType.Validation, failure.Errors.First().ErrorType);
        Assert.Equal("Email not in correct VIA format", failure.Errors.First().Message);
    }
}