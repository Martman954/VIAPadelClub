using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Common.Values;

public sealed record CourtId
{
    private string Value { get; }

    private CourtId(string value)
    {
        Value = value;
    }

    public static Result<CourtId> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<CourtId>(
                new ResultError("Court id cannot be null, empty, or whitespace.", ErrorType.Validation));
        }

        return new CourtId(value);
    }

    public override string ToString() => Value;
}