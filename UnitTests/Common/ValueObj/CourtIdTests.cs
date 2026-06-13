using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.CourtTests;

public class CourtIdTests
{
    [Theory]
    [InlineData("S1")]
    [InlineData("d5")]
    [InlineData("D10")]
    public void Create_WithValidString_ReturnsSuccessWithPopulatedValue(string validStringId)
    {
        // Act
        var result = CourtId.Create(validStringId);
        
        if (result is Result<CourtId>.Failure failure)
        {
            var errors = string.Join(", ", failure.Errors.Select(e => e.Message));
            throw new Xunit.Sdk.XunitException($"CourtId validation unexpectedly failed for '{validStringId}': {errors}");
        }

        // Assert
        Assert.True(result is Result<CourtId>.Success);
        if (result is Result<CourtId>.Success success)
        {
            Assert.Equal(validStringId.ToUpper(), success.Value.Value);
        }
    }

    [Theory]
    // Length violations
    [InlineData("S")]       // Too short
    [InlineData("D100")]    // Too long
    // Prefix violations
    [InlineData("C1")]      // Doesn't start with S or D
    [InlineData("A2")]      // Doesn't start with S or D
    // Suffix / Number violations
    [InlineData("SX")]      // Doesn't end with a number
    [InlineData("D0")]      // Number is below 1
    [InlineData("S11")]     // Number is above 10
    // Empty strings
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidFormats_ReturnsFailure(string? invalidStringId)
    {
        // Act
        if (invalidStringId != null)
        {
            var result = CourtId.Create(invalidStringId);

            // Assert
            Assert.True(result is Result<CourtId>.Failure, $"Expected validation failure for invalid input: '{invalidStringId}'");
        }
    }
}