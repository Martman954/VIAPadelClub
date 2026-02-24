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








