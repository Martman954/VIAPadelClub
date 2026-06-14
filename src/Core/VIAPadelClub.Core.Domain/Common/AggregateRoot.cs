namespace VIAPadelClub.Core.Domain.Common;

/// <summary>
/// Base class for all aggregate roots in the domain model.
/// An aggregate root is the entry point to an aggregate and acts as a consistency boundary.
/// </summary>
/// <typeparam name="TId">The type of the aggregate root's identifier</typeparam>
public abstract class AggregateRoot<TId> where TId : Id<TId>
{
    /// <summary>
    /// The unique identifier for this aggregate root
    /// </summary>
    public TId Id { get; protected set; } = null!;
}

