using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Application.AppEntry;

public sealed class Dispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
{
    public Task<Result> DispatchAsync<TCommand>(TCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        var handlerService = serviceProvider.GetService(typeof(ICommandHandler<TCommand>));

        if (handlerService is not ICommandHandler<TCommand> handler)
            throw new InvalidOperationException(
                $"No command handler is registered for '{typeof(TCommand).FullName}'.");

        return handler.HandleAsync(command);
    }
}

