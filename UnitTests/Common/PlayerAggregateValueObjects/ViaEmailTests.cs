using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;


namespace UnitTests.Common.PlayerAggregateValueObjects;

public class ViaEmailTests
{
    [Fact]
    public void CreateEmail_Should_ReturnSuccess_When_EmailIsValid()
    {
        // Act
        var result = ViaEmail.CreateEmail("test@test.com");

        // Assert
        var email = Assert.IsType<Result<ViaEmail>.Success>(result).Value;

        Assert.Equal("test@test.com", email.Value);
    }

    [Fact]
    public void CreateEmail_Should_ReturnFailure_When_EmailIsNull()
    {
        var result = ViaEmail.CreateEmail(null!);

        var failure = Assert.IsType<Result<ViaEmail>.Failure>(result);

        Assert.Single(failure.Errors);
        Assert.Equal(ErrorType.Validation, failure.Errors.First().ErrorType);
    }

    [Fact]
    public void CreateEmail_Should_ReturnFailure_When_EmailIsEmpty()
    {
        var result = ViaEmail.CreateEmail("");

        var failure = Assert.IsType<Result<ViaEmail>.Failure>(result);

        Assert.Single(failure.Errors);
    }

    [Fact]
    public void CreateEmail_Should_ReturnFailure_When_EmailDoesNotContainAtSymbol()
    {
        var result = ViaEmail.CreateEmail("invalidemail.com");

        var failure = Assert.IsType<Result<ViaEmail>.Failure>(result);

        Assert.Single(failure.Errors);
        Assert.Equal("Email not in correct format",
            failure.Errors.First().Message);
    }
}