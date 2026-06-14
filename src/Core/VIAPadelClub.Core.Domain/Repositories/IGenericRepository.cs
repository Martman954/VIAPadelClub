using VIAPadelClub.Core.Domain.Common;

namespace VIAPadelClub.Core.Domain.Repositories;

/// <summary>
/// Generic repository interface for managing aggregates.
/// Provides async CRUD operations for any aggregate root that inherits from AggregateRoot&lt;TId&gt;.
/// </summary>
/// <typeparam name="TAggr">The aggregate root type. Must inherit from AggregateRoot&lt;TId&gt;</typeparam>
/// <typeparam name="TId">The identifier type. Must inherit from Id&lt;TId&gt;</typeparam>
public interface IGenericRepository<TAggr, TId>
    where TAggr : AggregateRoot<TId>
    where TId : Id<TId>
{
    /// <summary>
    /// Retrieves an aggregate by its identifier with all dependent entities loaded
    /// </summary>
    /// <param name="id">The identifier of the aggregate to retrieve</param>
    /// <returns>A task representing the async operation, returning the aggregate or null if not found</returns>
    Task<TAggr?> GetAsync(TId id);

    /// <summary>
    /// Adds a new aggregate to the repository
    /// </summary>
    /// <param name="aggregate">The aggregate to add</param>
    /// <returns>A task representing the async operation</returns>
    Task AddAsync(TAggr aggregate);

    /// <summary>
    /// Removes an aggregate from the repository
    /// </summary>
    /// <param name="id">The identifier of the aggregate to remove</param>
    /// <returns>A task representing the async operation</returns>
    Task RemoveAsync(TId id);
}

