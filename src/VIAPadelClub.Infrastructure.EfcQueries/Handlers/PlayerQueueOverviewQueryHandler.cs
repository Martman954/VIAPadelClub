using VIAPadelClub.Core.QueryContracts;
using VIAPadelClub.Core.QueryContracts.Queries;

namespace VIAPadelClub.Infrastructure.EfcQueries.Handlers;

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

