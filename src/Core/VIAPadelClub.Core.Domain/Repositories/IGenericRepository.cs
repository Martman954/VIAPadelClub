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
    Task<TAggr?> GetAsync(TId id);
    
    Task AddAsync(TAggr aggregate);
    
    Task RemoveAsync(TId id);
}

