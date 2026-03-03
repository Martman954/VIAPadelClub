using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Tools.OperationResult.Results;


// Helper class, so you dont have to write "Result<int>.Success(object)" everytime
public abstract record Result
{
    // Helper methods: e.g. if something is "Result.Success()" it is automatically "Result<None>.Sucess(None.Value)"
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
            .OfType<Result<None>.Failure>()
            .SelectMany(f => f.Errors)
            .ToList();
        
        return errors.Any()
            ? Failure<None>(errors)
            : Success();
        
    }


    
    
}

// Factory class
public abstract record Result<T> : Result
{

    // Implicit operator returns Result in 2 formats: Success or Failure. Both are Result object with T type value
    public new sealed record Success(T Value) : Result<T>;

    public new sealed record Failure(IEnumerable<ResultError> Errors) : Result<T>
    {
        public Failure(ResultError error) : this(new[] { error }) { }
    }

    
    
    
    // Implicit operator so if Result<> has values its automatically a success
    public static implicit operator Result<T>(T value)
        => new Success(value);

    // Implicit operator so if Result<> is created and has error its automatically a Failure
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
}
public sealed record None
{
    public static readonly None Value = new();
}







