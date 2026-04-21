
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Domain.Common;

public interface ICommandHandler<TCommand>
{
    Task<Result> HandleAsync(TCommand command);
}