namespace VIAPadelClub.Core.Domain.Common;

public abstract class AggregateRoot<TId> where TId : Id<TId>
{
    public TId Id { get; protected set; } = null!;
}

