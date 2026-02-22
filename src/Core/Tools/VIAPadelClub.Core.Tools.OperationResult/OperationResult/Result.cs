using VIAPadelClub.Core.Tools.OperationResult.OperationResult.Errors;

namespace VIAPadelClub.Core.Tools.OperationResult.OperationResult;

public class Result
{
    public bool IsSuccess { get; set; }
    public bool isFailure => !IsSuccess;
    public List<Error> Errors { get; set; }

    protected Result(bool isSuccess, IEnumerable<Error>? errors = null)
    {
        IsSuccess = isSuccess;
        Errors = errors?.ToList() ?? new List<Error>();
    }

    public static Result Success() => new Result(true);
    public static Result Failure(Error error) => new(false, new[] { error });
    public static Result Failure(IEnumerable<Error> errors) => new(false, errors);

    public static Result Combine(params Result[] results)
    {
        var failures = results.Where(r => r.isFailure).SelectMany(r => r.Errors).ToList();
        return failures.Any() ? Failure(failures) : Success();
    }
    

}