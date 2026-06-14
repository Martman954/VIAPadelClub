using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Repositories;

namespace UnitTests.Fakes;

internal class FakePlayerRepo : IPlayerRepository
{
    public List<Player> Players { get; } = [];

    public Task AddAsync(Player player)
    {
        Players.Add(player);
        return Task.CompletedTask;
    }

    public Task<Player?> GetAsync(ViaEmail playerId)
        => Task.FromResult(Players.FirstOrDefault(p => p.Email.Value == playerId.Value));

    public Task RemoveAsync(ViaEmail playerId)
    {
        var player = Players.FirstOrDefault(p => p.Email.Value == playerId.Value);
        if (player != null)
            Players.Remove(player);
        return Task.CompletedTask;
    }
}