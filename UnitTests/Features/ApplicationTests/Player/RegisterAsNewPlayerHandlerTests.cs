using VIAPadelClub.Core.Application.AppEntry.PlayerCommands;
using VIAPadelClub.Core.Application.Features.Players;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Players;
using VIAPadelClub.Core.Domain.Repositories;
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

file class FakePlayerRepo : IPlayerRepository
{
    public List<VIAPadelClub.Core.Domain.Aggregates.Players.Player> Players { get; } = [];

    public Task AddAsync(VIAPadelClub.Core.Domain.Aggregates.Players.Player player)
    {
        Players.Add(player);
        return Task.CompletedTask;
    }

    public Task<VIAPadelClub.Core.Domain.Aggregates.Players.Player?> GetAsync(ViaEmail playerId)
        => Task.FromResult(Players.FirstOrDefault(p => p.Email.Value == playerId.Value));

    public Task RemoveAsync(ViaEmail playerId)
    {
        var player = Players.FirstOrDefault(p => p.Email.Value == playerId.Value);
        if (player != null)
            Players.Remove(player);
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
        var handler = new RegisterAsNewPlayerHandler(repo, new EmailAvailableChecker());

        var result = await handler.HandleAsync(ValidCommand());

        Assert.IsType<Result<None>.Success>(result);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_PlayerIsAddedToRepo()
    {
        var repo = new FakePlayerRepo();
        var handler = new RegisterAsNewPlayerHandler(repo, new EmailAvailableChecker());

        await handler.HandleAsync(ValidCommand());

        Assert.Single(repo.Players);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_PlayerHasCorrectEmail()
    {
        var repo = new FakePlayerRepo();
        var handler = new RegisterAsNewPlayerHandler(repo, new EmailAvailableChecker());

        await handler.HandleAsync(ValidCommand());

        Assert.Equal(ValidEmail, repo.Players[0].Email.Value);
    }

    [Fact]
    public async Task HandleAsync_EmailAlreadyInUse_PlayerNotAddedToRepo()
    {
        var repo = new FakePlayerRepo();
        var handler = new RegisterAsNewPlayerHandler(repo, new EmailInUseChecker());

        await handler.HandleAsync(ValidCommand());

        Assert.Empty(repo.Players);
    }

    [Fact]
    public async Task HandleAsync_EmailAlreadyInUse_ReturnsFailure()
    {
        var repo = new FakePlayerRepo();
        var handler = new RegisterAsNewPlayerHandler(repo, new EmailInUseChecker());

        var result = await handler.HandleAsync(ValidCommand());

        Assert.IsType<Result<None>.Failure>(result);
    }
}