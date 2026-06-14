namespace VIAPadelClub.Core.QueryContracts;

public static class PlayerQueueOverview
{
    public sealed record Query(string PlayerIdOrEmail);

    public sealed record Answer(
        IReadOnlyList<QueueItem> Items);

    public sealed record QueueItem(
        string QueueId,
        DateOnly Date,
        string CourtId,
        TimeOnly RequestedAt);
}