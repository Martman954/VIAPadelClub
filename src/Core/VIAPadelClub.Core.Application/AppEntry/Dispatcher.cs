using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Application.AppEntry;

/// <summary>
/// Resolves the matching command handler from DI and forwards the command to it.
/// </summary>
public sealed class Dispatcher(IServiceProvider serviceProvider) : ICommandDispatch
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

