using System.Text.RegularExpressions;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Common.Values;

public class ViaEmail
{
    public string Value { get; }
    private static readonly Regex EmailRegex =
        new(@"^([a-zA-Z]{3,4}|\d{6})@via\.dk$", RegexOptions.Compiled);

    private ViaEmail(string email)
    {
        Value = email;
    }

    public static Result<ViaEmail> CreateEmail(string email)
        =>
            Result.Combine(
                EmailMustBeCorrectFormat(email)
            ).WithSuccessPayload(new ViaEmail(email));

    private static Result<None> EmailMustBeCorrectFormat(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure("Email is empty", ErrorType.Validation);

        var normalized = email.ToLowerInvariant();

        if (!EmailRegex.IsMatch(normalized))
            return Result.Failure("Email not in correct VIA format", ErrorType.Validation);

        return Result.Success();
    }
}