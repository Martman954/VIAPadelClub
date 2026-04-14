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

    public static Result<CourtId> Create(string courtId)
    {
        if (string.IsNullOrWhiteSpace(courtId))
            return Result.Failure<CourtId>(new ResultError(
                "Court name cannot be empty.",
                ErrorType.Validation));

        var normalized = courtId.ToUpper();

        if (normalized.Length < 2 || normalized.Length > 3)
            return Result.Failure<CourtId>(new ResultError(
                "Court name must be 2 or 3 characters long.",
                ErrorType.Validation));

        if (normalized[0] != 'S' && normalized[0] != 'D')
            return Result.Failure<CourtId>(new ResultError(
                "Court name must start with 'S' or 'D'.",
                ErrorType.Validation));

        if (!int.TryParse(normalized[1..], out int number))
            return Result.Failure<CourtId>(new ResultError(
                "Court name must end with a number.",
                ErrorType.Validation));

        if (number < 1 || number > 10)
            return Result.Failure<CourtId>(new ResultError(
                "Court number must be between 1 and 10.",
                ErrorType.Validation));

        return Result.Success(new CourtId(normalized));
    }

    public override string ToString() => Value;
}