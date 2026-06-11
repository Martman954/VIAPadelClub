
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Application.CommandDispatch;

public interface ICommandHandler<in TCommand>
{
    Task<Result> HandleAsync(TCommand command);
}