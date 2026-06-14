namespace VIAPadelClub.Core.QueryContracts;

public sealed class QueryDispatcher(IServiceProvider serviceProvider) : IQueryDispatcher
{
    public Task<TAnswer> DispatchAsync<TAnswer>(IQuery<TAnswer> query)
    {
        ArgumentNullException.ThrowIfNull(query);

        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TAnswer));
        var handlerService = serviceProvider.GetService(handlerType);

        if (handlerService == null)
            throw new InvalidOperationException(
                $"No query handler is registered for '{query.GetType().FullName}'.");

        var handleMethod = handlerType.GetMethod(nameof(IQueryHandler<IQuery<TAnswer>, TAnswer>.HandleAsync))
            ?? throw new InvalidOperationException(
                $"Query handler '{handlerType.FullName}' has no HandleAsync method.");

        var task = handleMethod.Invoke(handlerService, [query]) as Task<TAnswer>
            ?? throw new InvalidOperationException(
                $"Query handler '{handlerType.FullName}' returned null task.");

        return task;
    }
}

