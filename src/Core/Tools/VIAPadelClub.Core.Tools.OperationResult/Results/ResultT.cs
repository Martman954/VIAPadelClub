using VIAPadelClub.Core.Tools.OperationResult.Entities;

namespace VIAPadelClub.Core.Tools.OperationResult.Results.Errors;


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