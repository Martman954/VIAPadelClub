using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Tools.OperationResult.Results;

public static class ResultExtensions
{
    public static Result<T> WithSuccessPayload<T>(this Result<None> result, T payload)
        => result switch
        {
            Result<None>.Success => Result.Success(payload),
            Result<None>.Failure f => Result.Failure<T>(f.Errors),
            _ => throw new InvalidOperationException("Unknown result type")
        };
}