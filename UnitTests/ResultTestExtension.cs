using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace UnitTests;

public static class ResultTestExtensions
{
    public static T AssertSuccess<T>(this Result<T> result)
    {
        var success = Assert.IsType<Result<T>.Success>(result);
        return success.Value;
    }

    public static IEnumerable<ResultError> AssertFailure<T>(this Result<T> result)
    {
        var failure = Assert.IsType<Result<T>.Failure>(result);
        return failure.Errors;
    }
}