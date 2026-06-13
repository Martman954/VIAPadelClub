using VIAPadelClub.Core.Application.AppEntry.PlayerCommands;
using VIAPadelClub.Core.Application.Features.Players;
using UnitTests.Fakes;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.PlayerTests.RegisterPlayer;

public class RegisterAsNewPlayerHandlerTests
{
    private const string ValidEmail     = "123456@via.dk";
    private const string ValidFirstName = "Alex";
    private const string ValidLastName  = "Andersen";
    private const string ValidImageUrl  = "https://via.dk/pic.png";

    private static RegisterAsNewPlayerCommand ValidCommand()
        => ((Result<RegisterAsNewPlayerCommand>.Success)
            RegisterAsNewPlayerCommand.Create(ValidEmail, ValidFirstName, ValidLastName, ValidImageUrl)).Value;

    [Fact]
    public async Task GivenValidCommand_WhenHandlingAsync_ThenReturnsSuccess()
    {
        var repo    = new FakePlayerRepo();
        var uow     = new FakeUnitOfWork();
        var handler = new RegisterAsNewPlayerHandler(repo, uow, new FakeEmailChecker());

        var result = await handler.HandleAsync(ValidCommand());

        Assert.IsType<Result<None>.Success>(result);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandlingAsync_ThenPlayerIsAddedToRepo()
    {
        var repo    = new FakePlayerRepo();
        var uow     = new FakeUnitOfWork();
        var handler = new RegisterAsNewPlayerHandler(repo, uow, new FakeEmailChecker());

        await handler.HandleAsync(ValidCommand());

        Assert.Single(repo.Players);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandlingAsync_ThenPlayerHasCorrectEmail()
    {
        var repo    = new FakePlayerRepo();
        var uow     = new FakeUnitOfWork();
        var handler = new RegisterAsNewPlayerHandler(repo, uow, new FakeEmailChecker());

        await handler.HandleAsync(ValidCommand());

        Assert.Equal(ValidEmail, repo.Players[0].Email.Value);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandlingAsync_ThenSaveChangesIsCalled()
    {
        var repo    = new FakePlayerRepo();
        var uow     = new FakeUnitOfWork();
        var handler = new RegisterAsNewPlayerHandler(repo, uow, new FakeEmailChecker());

        await handler.HandleAsync(ValidCommand());

        Assert.True(uow.SaveChangesCalled);
    }

    [Fact]
    public async Task GivenEmailAlreadyInUse_WhenHandlingAsync_ThenPlayerNotAddedToRepo()
    {
        var repo    = new FakePlayerRepo();
        var uow     = new FakeUnitOfWork();
        var handler = new RegisterAsNewPlayerHandler(repo, uow, new EmailInUseChecker());

        try { await handler.HandleAsync(ValidCommand()); }
        catch { /* ignored */ }

        Assert.Empty(repo.Players);
    }

    [Fact]
    public async Task GivenEmailAlreadyInUse_WhenHandlingAsync_ThenSaveChangesNotCalled()
    {
        var repo    = new FakePlayerRepo();
        var uow     = new FakeUnitOfWork();
        var handler = new RegisterAsNewPlayerHandler(repo, uow, new EmailInUseChecker());

        try { await handler.HandleAsync(ValidCommand()); }
        catch { /* ignored */ }

        Assert.False(uow.SaveChangesCalled);
    }
}