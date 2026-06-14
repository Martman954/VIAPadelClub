namespace VIAPadelClub.Core.QueryContracts;

public interface IQueryHandler<TQuery, TAnswer>
{
    public Task<TAnswer> HandleAsync(TQuery query);
}