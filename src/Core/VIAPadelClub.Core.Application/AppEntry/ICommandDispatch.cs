using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Application.AppEntry;

public interface ICommandDispatch
{
    Task<Result> DispatchAsync<TCommand>(TCommand command);
}