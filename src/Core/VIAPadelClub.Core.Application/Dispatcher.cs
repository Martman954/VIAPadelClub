using VIAPadelClub.Core.Application.Abstractions;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Application;

public class Dispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
{
    public Task<Result> DispatchAsync<TCommand>(TCommand command)
    {
        Type serviceType = typeof(ICommandHandler<TCommand>);
        
        var service = serviceProvider.GetService(serviceType);

        if (service == null)
        {
            throw new InvalidOperationException($"No handler found for {typeof(TCommand).Name}");
        }
        
        ICommandHandler<TCommand> handler = (ICommandHandler<TCommand>)service;
        return handler.HandleAsync(command);
    }
}