using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Application.Abstractions;

public interface ICommandHandler<in TCommand>
{
    Task<Result> HandleAsync(TCommand command);
}