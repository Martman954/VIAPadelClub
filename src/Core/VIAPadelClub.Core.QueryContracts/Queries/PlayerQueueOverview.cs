namespace VIAPadelClub.Core.QueryContracts.Queries;

public static class PlayerQueueOverview
{
    public sealed record Query(string PlayerIdOrEmail) : IQuery<Answer>;

    public sealed record Answer(
        IReadOnlyList<QueueItem> Items);

    public sealed record QueueItem(
        string QueueId,
        DateOnly Date,
        string CourtId,
        TimeOnly RequestedAt);
}