using VIAPadelClub.Core.QueryContracts;

namespace VIAPadelClub.Infrastructure.EfcQueries;

public sealed class PlayerQueueOverviewQueryHandler
    : IQueryHandler<PlayerQueueOverview.Query, PlayerQueueOverview.Answer>
{
    public Task<PlayerQueueOverview.Answer> HandleAsync(PlayerQueueOverview.Query query)
    {
        // Queue read model is not implemented yet in the domain/persistence layer.
        // Returning an empty projection keeps the query side stable for UI integration.
        return Task.FromResult(new PlayerQueueOverview.Answer([]));
    }
}

