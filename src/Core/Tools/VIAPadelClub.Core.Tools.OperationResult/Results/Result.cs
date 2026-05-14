using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Tools.OperationResult.Results;

public abstract record Result
{
    internal abstract IEnumerable<ResultError> GetErrors();

    public static Result<None> Success()
        => new Result<None>.Success(None.Value);

    public static Result<T> Success<T>(T value)
        => new Result<T>.Success(value);

    public static Result<None> Failure(string error, ErrorType errorType)
        => new Result<None>.Failure(new ResultError(error, errorType));

    public static Result<T> Failure<T>(ResultError error)
        => new Result<T>.Failure(error);

    public static Result<T> Failure<T>(IEnumerable<ResultError> errors)
        => new Result<T>.Failure(errors);

    public static Result<None> Combine(params Result[] results)
    {
        IEnumerable<ResultError> errors = results
            .SelectMany(r => r.GetErrors())
            .ToList();

        return errors.Any()
            ? Failure<None>(errors)
            : Success();
    }

    public static Result<TCommand> CombineResultsInto<TCommand>(params Result[] results)
    {
        IEnumerable<ResultError> errors = results
            .SelectMany(r => r.GetErrors())
            .ToList();

        return errors.Any()
            ? Failure<TCommand>(errors)
            : Success(default(TCommand)!);
    }
}

public abstract record Result<T> : Result
{
    public new sealed record Success(T Value) : Result<T>;

    public new sealed record Failure(IEnumerable<ResultError> Errors) : Result<T>
    {
        public Failure(ResultError error) : this(new[] { error }) { }
    }

    internal override IEnumerable<ResultError> GetErrors() => this switch
    {
        Failure f => f.Errors,
        _ => Enumerable.Empty<ResultError>()
    };

    public T Payload => this is Success s
        ? s.Value
        : throw new InvalidOperationException("Cannot access Payload on a failed result.");

    public static implicit operator Result<T>(T value)
        => new Success(value);

    public static implicit operator Result<T>(ResultError error)
        => new Failure(error);
}

public static class ResultExtensions
{
    public static Result<T> WithSuccessPayload<T>(this Result<None> result, T payload)
        => result switch
        {
            Result<None>.Success => Result.Success(payload),
            Result<None>.Failure f => Result.Failure<T>(f.Errors),
            _ => throw new InvalidOperationException("Unknown result type")
        };

    public static Result<TCommand> WithPayloadIfSuccess<TCommand>(
        this Result<TCommand> result, Func<TCommand> factory)
        => result switch
        {
            Result<TCommand>.Success => Result.Success(factory()),
            Result<TCommand>.Failure f => Result.Failure<TCommand>(f.Errors),
            _ => throw new InvalidOperationException("Unknown result type")
        };
}

public sealed record None
{
    public static readonly None Value = new();
}