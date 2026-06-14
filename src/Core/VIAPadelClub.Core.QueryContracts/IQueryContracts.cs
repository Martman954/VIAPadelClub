namespace VIAPadelClub.Core.QueryContracts;

public interface IQuery<TAnswer>;

public interface IQueryHandler<in TQuery, TAnswer>
    where TQuery : IQuery<TAnswer>
{
    public Task<TAnswer> HandleAsync(TQuery query);
}

public interface IQueryDispatcher
{
    Task<TAnswer> DispatchAsync<TAnswer>(IQuery<TAnswer> query);
}
