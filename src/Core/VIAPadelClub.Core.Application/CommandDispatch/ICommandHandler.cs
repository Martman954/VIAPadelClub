
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.CommandDispatch;

public interface ICommandHandler<in TCommand>
{
    Task<Result> HandleAsync(TCommand command);
}