using VIAPadelClub.Core.Application.CommandDispatch.PlayerCommands;
using VIAPadelClub.Core.Application.Features.Players;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Players;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.ApplicationTests.Player;

file class EmailAvailableChecker : IEmailInUseChecker
{
    public bool IsEmailInUse(ViaEmail email) => false;
}

file class EmailInUseChecker : IEmailInUseChecker
{
    public bool IsEmailInUse(ViaEmail email) => true;
}

file class FakePlayerRepo : IPlayerRepo
{
    public List<VIAPadelClub.Core.Domain.Aggregates.Players.Player> Players { get; } = [];

    public Task<VIAPadelClub.Core.Domain.Aggregates.Players.Player> AddPlayer(VIAPadelClub.Core.Domain.Aggregates.Players.Player player)
    {
        Players.Add(player);
        return Task.FromResult(player);
    }

    public Task<VIAPadelClub.Core.Domain.Aggregates.Players.Player> GetPlayer(Guid playerId)
        => Task.FromResult(Players.First(p => p.Email.Value == playerId.ToString()));

    public Task<VIAPadelClub.Core.Domain.Aggregates.Players.Player> RemovePlayer(Guid playerId)
    {
        var player = Players.First(p => p.Email.Value == playerId.ToString());
        Players.Remove(player);
        return Task.FromResult(player);
    }
}

file class FakeUnitOfWork : IUnitOfWork
{
    public bool SaveChangesCalled { get; private set; }

    public Task SaveChangesAsync()
    {
        SaveChangesCalled = true;
        return Task.CompletedTask;
    }
}

public class ResgisterAsNewPlayerHandlerTests
{
    private const string ValidEmail     = "123456@via.dk";
    private const string ValidFirstName = "Alex";
    private const string ValidLastName  = "Andersen";
    private const string ValidImageUrl  = "https://via.dk/pic.png";

    private static RegisterAsNewPlayerCommand ValidCommand()
        => ((Result<RegisterAsNewPlayerCommand>.Success)
            RegisterAsNewPlayerCommand.Create(ValidEmail, ValidFirstName, ValidLastName, ValidImageUrl)).Value;

    [Fact]
    public void CreateCommand_ValidInputs_ReturnsSuccess()
    {
        var result = RegisterAsNewPlayerCommand.Create(ValidEmail, ValidFirstName, ValidLastName, ValidImageUrl);

        Assert.IsType<Result<RegisterAsNewPlayerCommand>.Success>(result);
    }

    [Theory]
    [InlineData("invalid", "Alex", "Andersen", "https://via.dk/pic.png")]
    [InlineData("123456@via.dk", "", "Andersen", "https://via.dk/pic.png")]
    [InlineData("123456@via.dk", "Alex", "", "https://via.dk/pic.png")]
    [InlineData("123456@via.dk", "Alex", "Andersen", "not-a-url")]
    public void CreateCommand_InvalidInputs_ReturnsFailure(string email, string first, string last, string image)
    {
        var result = RegisterAsNewPlayerCommand.Create(email, first, last, image);

        Assert.IsType<Result<RegisterAsNewPlayerCommand>.Failure>(result);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_ReturnsSuccess()
    {
        var repo = new FakePlayerRepo();
        var uow  = new FakeUnitOfWork();
        var handler = new RegisterAsNewPlayerHandler(repo, uow, new EmailAvailableChecker());

        var result = await handler.HandleAsync(ValidCommand());

        Assert.IsType<Result<None>.Success>(result);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_PlayerIsAddedToRepo()
    {
        var repo = new FakePlayerRepo();
        var uow  = new FakeUnitOfWork();
        var handler = new RegisterAsNewPlayerHandler(repo, uow, new EmailAvailableChecker());

        await handler.HandleAsync(ValidCommand());

        Assert.Single(repo.Players);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_PlayerHasCorrectEmail()
    {
        var repo = new FakePlayerRepo();
        var uow  = new FakeUnitOfWork();
        var handler = new RegisterAsNewPlayerHandler(repo, uow, new EmailAvailableChecker());

        await handler.HandleAsync(ValidCommand());

        Assert.Equal(ValidEmail, repo.Players[0].Email.Value);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_SaveChangesIsCalled()
    {
        var repo = new FakePlayerRepo();
        var uow  = new FakeUnitOfWork();
        var handler = new RegisterAsNewPlayerHandler(repo, uow, new EmailAvailableChecker());

        await handler.HandleAsync(ValidCommand());

        Assert.True(uow.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_EmailAlreadyInUse_PlayerNotAddedToRepo()
    {
        var repo = new FakePlayerRepo();
        var uow  = new FakeUnitOfWork();
        var handler = new RegisterAsNewPlayerHandler(repo, uow, new EmailInUseChecker());

        try { await handler.HandleAsync(ValidCommand()); }
        catch
        {
            // ignored
        }

        Assert.Empty(repo.Players);
    }

    [Fact]
    public async Task HandleAsync_EmailAlreadyInUse_SaveChangesNotCalled()
    {
        var repo = new FakePlayerRepo();
        var uow  = new FakeUnitOfWork();
        var handler = new RegisterAsNewPlayerHandler(repo, uow, new EmailInUseChecker());

        try { await handler.HandleAsync(ValidCommand()); }
        catch
        {
            // ignored
        }

        Assert.False(uow.SaveChangesCalled);
    }
}