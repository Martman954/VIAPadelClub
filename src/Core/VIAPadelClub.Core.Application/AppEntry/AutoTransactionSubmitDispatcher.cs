using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Application.AppEntry;

/// <summary>
/// Decorator that commits UnitOfWork when command handling succeeds.
/// </summary>
public sealed class AutoTransactionSubmitDispatcher(
    ICommandDispatcher next,
    IUnitOfWork unitOfWork) : ICommandDispatcher
{
    public async Task<Result> DispatchAsync<TCommand>(TCommand command)
    {
        var result = await next.DispatchAsync(command);

        if (result is Result<None>.Success)
            await unitOfWork.SaveChangesAsync();

        return result;
    }
}

