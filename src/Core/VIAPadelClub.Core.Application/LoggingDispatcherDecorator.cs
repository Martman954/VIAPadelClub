using VIAPadelClub.Core.Application.Abstractions;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Application;

public class LoggingDispatcherDecorator : ICommandDispatcher
{
    private readonly ICommandDispatcher? next;

    public LoggingDispatcherDecorator(ICommandDispatcher? next)
        => this.next = next;

    public async Task<Result> DispatchAsync<TCommand>(TCommand command)
    {
        Console.WriteLine($"[LOG] Starting command: {typeof(TCommand).Name}");
        
        var result = next is not null 
            ? await next.DispatchAsync(command) 
            : Result.Success();
        
        Console.WriteLine($"[LOG] Finished command: {typeof(TCommand).Name}");

        return result;
    }
}