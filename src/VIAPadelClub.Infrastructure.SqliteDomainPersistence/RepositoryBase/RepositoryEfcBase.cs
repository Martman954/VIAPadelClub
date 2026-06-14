using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Domain.Repositories;

namespace VIAPadelClub.Infrastructure.SqliteDomainPersistence.RepositoryBase;

/// <summary>
/// Abstract base class for Entity Framework Core repository implementations.
/// Provides common async CRUD operations for aggregates.
/// </summary>
/// <typeparam name="TAgg">The aggregate root type</typeparam>
/// <typeparam name="TId">The identifier type</typeparam>
public abstract class RepositoryEfcBase<TAgg, TId> : IGenericRepository<TAgg, TId>
    where TAgg : AggregateRoot<TId>
    where TId : Id<TId>
{
    /// <summary>
    /// The DbContext used for database operations
    /// </summary>
    protected readonly DbContext Context;

    /// <summary>
    /// Initializes a new instance of the repository with a DbContext
    /// </summary>
    /// <param name="context">The DbContext to use for database operations</param>
    protected RepositoryEfcBase(DbContext context)
    {
        Context = context;
    }

    /// <summary>
    /// Retrieves an aggregate by its identifier with all dependent entities loaded.
    /// Override this method to include related entities using Include() or ThenInclude()
    /// </summary>
    /// <param name="id">The identifier of the aggregate to retrieve</param>
    /// <returns>The aggregate with the given id, or null if not found</returns>
    public virtual async Task<TAgg?> GetAsync(TId id)
    {
        return await Context.Set<TAgg>().FindAsync([id], cancellationToken: default);
    }

    /// <summary>
    /// Adds a new aggregate to the repository context.
    /// Note: Changes are not persisted until SaveChangesAsync is called on the UnitOfWork
    /// </summary>
    /// <param name="aggregate">The aggregate to add</param>
    public virtual async Task AddAsync(TAgg aggregate)
    {
        await Context.Set<TAgg>().AddAsync(aggregate);
    }

    /// <summary>
    /// Removes an aggregate from the repository context.
    /// Note: Changes are not persisted until SaveChangesAsync is called on the UnitOfWork
    /// </summary>
    /// <param name="id">The identifier of the aggregate to remove</param>
    public virtual async Task RemoveAsync(TId id)
    {
        var aggregate = await GetAsync(id);
        if (aggregate is not null)
        {
            Context.Set<TAgg>().Remove(aggregate);
        }
    }
}

