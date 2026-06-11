using Features.CommandDispatch.PlayerCommands;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.PlayerTests.RegisterPlayer;

public class RegisterAsNewPlayerCommandTests
{
    private const string ValidEmail     = "123456@via.dk";
    private const string ValidFirstName = "Alex";
    private const string ValidLastName  = "Andersen";
    private const string ValidImageUrl  = "https://via.dk/pic.png";

    [Fact]
    public void Create_ValidInputs_ReturnsSuccess()
    {
        var result = RegisterAsNewPlayerCommand.Create(ValidEmail, ValidFirstName, ValidLastName, ValidImageUrl);

        Assert.IsType<Result<RegisterAsNewPlayerCommand>.Success>(result);
    }

    [Fact]
    public void Create_ValidInputs_CommandHasCorrectEmail()
    {
        var result = RegisterAsNewPlayerCommand.Create(ValidEmail, ValidFirstName, ValidLastName, ValidImageUrl);

        var success = Assert.IsType<Result<RegisterAsNewPlayerCommand>.Success>(result);
        Assert.Equal(ValidEmail, success.Value.Email.Value);
    }

    [Fact]
    public void Create_ValidInputs_CommandHasCorrectName()
    {
        var result = RegisterAsNewPlayerCommand.Create(ValidEmail, ValidFirstName, ValidLastName, ValidImageUrl);

        var success = Assert.IsType<Result<RegisterAsNewPlayerCommand>.Success>(result);
        Assert.Equal(ValidFirstName, success.Value.Name.FirstName);
        Assert.Equal(ValidLastName, success.Value.Name.LastName);
    }

    [Fact]
    public void Create_ValidInputs_CommandHasCorrectImageUrl()
    {
        var result = RegisterAsNewPlayerCommand.Create(ValidEmail, ValidFirstName, ValidLastName, ValidImageUrl);

        var success = Assert.IsType<Result<RegisterAsNewPlayerCommand>.Success>(result);
        Assert.Equal(ValidImageUrl, success.Value.ImageUrl.Value);
    }

    [Fact]
    public void Create_InvalidEmail_ReturnsFailure()
    {
        var result = RegisterAsNewPlayerCommand.Create("not-an-email", ValidFirstName, ValidLastName, ValidImageUrl);

        Assert.IsType<Result<RegisterAsNewPlayerCommand>.Failure>(result);
    }

    [Fact]
    public void Create_EmptyFirstName_ReturnsFailure()
    {
        var result = RegisterAsNewPlayerCommand.Create(ValidEmail, "", ValidLastName, ValidImageUrl);

        Assert.IsType<Result<RegisterAsNewPlayerCommand>.Failure>(result);
    }

    [Fact]
    public void Create_EmptyLastName_ReturnsFailure()
    {
        var result = RegisterAsNewPlayerCommand.Create(ValidEmail, ValidFirstName, "", ValidImageUrl);

        Assert.IsType<Result<RegisterAsNewPlayerCommand>.Failure>(result);
    }

    [Fact]
    public void Create_InvalidImageUrl_ReturnsFailure()
    {
        var result = RegisterAsNewPlayerCommand.Create(ValidEmail, ValidFirstName, ValidLastName, "not-a-url");

        Assert.IsType<Result<RegisterAsNewPlayerCommand>.Failure>(result);
    }

    [Fact]
    public void Create_MultipleInvalidInputs_ReturnsFailure()
    {
        var result = RegisterAsNewPlayerCommand.Create("bad-email", "", "", "bad-url");

        Assert.IsType<Result<RegisterAsNewPlayerCommand>.Failure>(result);
    }

    [Fact]
    public void Create_MultipleInvalidInputs_ReturnsAllErrors()
    {
        var result = RegisterAsNewPlayerCommand.Create("bad-email", "", "", "bad-url");

        var failure = Assert.IsType<Result<RegisterAsNewPlayerCommand>.Failure>(result);
        Assert.True(failure.Errors.Count() > 1);
    }
}