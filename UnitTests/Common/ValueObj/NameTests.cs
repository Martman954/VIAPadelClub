using VIAPadelClub.Core.Domain.Aggregates.Players.ValueObjects;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace UnitTests.Common.PlayerAggregateValueObjects;

public class NameTests
{

    [Fact]
    public void CreateName_ValidNames_ReturnsSuccess()
    {
        var name = Name.CreateName("John", "Doe").AssertSuccess();

        Assert.Equal("John", name.FirstName);
        Assert.Equal("Doe", name.LastName);
    }

    [Fact]
    public void CreateName_ValidNames_NormalizesCapitalization()
    {
        var name = Name.CreateName("jOHN", "dOE").AssertSuccess();

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
    public void CreateName_NullOrEmpty_ReturnsFailureWithConflictError(string first, string last)
    {
        var result = Name.CreateName(first!, last!);

        var errors = result.AssertFailure();
        Assert.All(errors, e => Assert.Equal(ErrorType.Conflict, e.ErrorType));
    }

    [Theory]
    [InlineData("J", "Doe")]        // first name too short
    [InlineData("John123", "Doe")]  // contains numbers
    [InlineData("John", "D")]       // last name too short
    [InlineData("John!", "Doe")]    // special characters
    public void CreateName_InvalidFormat_ReturnsFailureWithTeapotError(string first, string last)
    {
        var result = Name.CreateName(first!, last!);

        var errors = result.AssertFailure();
        Assert.All(errors, e => Assert.Equal(ErrorType.ImATeaPot, e.ErrorType));
    }
}