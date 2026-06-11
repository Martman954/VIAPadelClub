using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Repositories;

namespace UnitTests.Fakes;

internal class FakePlayerRepo : IPlayerRepo
{
    public List<Player> Players { get; } = [];

    public Task<Player> AddPlayer(Player player)
    {
        Players.Add(player);
        return Task.FromResult(player);
    }

    public Task<Player> GetPlayer(Guid playerId)
        => Task.FromResult(Players.First(p => p.Email.Value == playerId.ToString()));

    public Task<Player> RemovePlayer(Guid playerId)
    {
        var player = Players.First(p => p.Email.Value == playerId.ToString());
        Players.Remove(player);
        return Task.FromResult(player);
    }
}