using VIAPadelClub.Core.Tools.OperationResult.OperationResult.Errors;

namespace VIAPadelClub.Core.Tools.OperationResult.Result;

/// <summary>
/// Represents the result of an operation that returns a value of type T.
/// </summary>
/// <typeparam name="T">The type of the value contained within the result.</typeparam>
public class Result<T> : Result
{
    private readonly T? _value;

    public T Value =>
        IsSuccess ? _value! : throw new InvalidOperationException("Cannot access Value of a failed Result.");

    
    protected internal Result(T? value, bool isSuccess, IEnumerable<Error>? errors = null) : base(isSuccess, errors)
    {
        _value = value;
    }
    
    public static Result<T> Success(T value) => new(value, true);
    public new static Result<T> Failure(Error error) => new(default, false, new[] { error });
    public new static Result<T> Failure(IEnumerable<Error> errors) => new(default, false, errors);
    
    public static implicit operator Result<T> (T value) => Success(value);
    
    public static implicit operator Result<T> (Error error) => Failure(error);
    
    
}