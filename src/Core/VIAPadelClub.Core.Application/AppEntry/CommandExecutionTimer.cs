using System.Diagnostics;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Application.AppEntry;

public sealed class CommandExecutionTimer(ICommandDispatcher next) : ICommandDispatcher
{
    public async Task<Result> DispatchAsync<TCommand>(TCommand command)
    {
        var stopwatch = Stopwatch.StartNew();

        var result = await next.DispatchAsync(command);

        stopwatch.Stop();
        _ = stopwatch.Elapsed;

        return result;
    }
}

