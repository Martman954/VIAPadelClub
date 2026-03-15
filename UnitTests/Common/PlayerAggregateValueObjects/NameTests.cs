using VIAPadelClub.Core.Domain.Aggregates.Player.ValueObjects;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace UnitTests.Common.PlayerAggregateValueObjects;

public class NameTests
{
    [Fact]
    public void CreateName_Should_ReturnSuccess_When_NamesAreValid()
    {
        var name = Name.CreateName("John", "Doe")
            .AssertSuccess();

        Assert.Equal("John", name.FirstName);
        Assert.Equal("Doe", name.LastName);
    }

    [Theory]
    [InlineData(null, "Doe")]
    [InlineData("", "Doe")]
    [InlineData("   ", "Doe")]
    [InlineData("John", null)]
    [InlineData("John", "")]
    [InlineData("John", "   ")]
    public void CreateName_Should_ReturnFailure_When_NameInvalid(
        string first,
        string last)
    {
        var result = Name.CreateName(first!, last!);

        var errors = result.AssertFailure();

        Assert.All(errors,
            e => Assert.Equal(ErrorType.Validation, e.ErrorType));
    }
}