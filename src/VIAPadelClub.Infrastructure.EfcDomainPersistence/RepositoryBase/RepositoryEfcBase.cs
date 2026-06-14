using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Domain.Repositories;

namespace VIAPadelClub.Infrastructure.EfcDomainPersistence.RepositoryBase;

/// <summary>
/// Abstract base class for Entity Framework Core repository implementations.
/// Provides common async CRUD operations for aggregates.
/// </summary>
/// <typeparam name="TAgg">The aggregate root type</typeparam>
/// <typeparam name="TId">The identifier type</typeparam>
public abstract class RepositoryEfcBase<TAgg, TId>(DbContext context) : IGenericRepository<TAgg, TId>
    where TAgg : AggregateRoot<TId>
    where TId : Id<TId>
{
    protected readonly DbContext Context = context;
    
    public virtual async Task<TAgg?> GetAsync(TId id)
    {
        return await Context.Set<TAgg>().FindAsync([id], cancellationToken: default);
    }
    
    public virtual async Task AddAsync(TAgg aggregate)
    {
        await Context.Set<TAgg>().AddAsync(aggregate);
    }
    
    public virtual async Task RemoveAsync(TId id)
    {
        var aggregate = await GetAsync(id);
        if (aggregate is not null)
        {
            Context.Set<TAgg>().Remove(aggregate);
        }
    }
}

