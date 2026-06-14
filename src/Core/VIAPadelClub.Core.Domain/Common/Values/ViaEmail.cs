using System.Text.RegularExpressions;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Common.Values;

public sealed class ViaEmail : Id<ViaEmail>
{
    public string StringValue { get; }
    public override string Value => StringValue;
    private static readonly Regex EmailRegex =
        new(@"^([a-zA-Z]{3,4}|\d{6})@via\.dk$", RegexOptions.Compiled);

    private ViaEmail(string email)
    {
        StringValue = email;
    }

    public static Result<ViaEmail> CreateEmail(string email)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return Result.Combine(
                EmailMustBeCorrectFormat(normalized)
            )
            .WithSuccessPayload(new ViaEmail(normalized));
    }

    private static Result<None> EmailMustBeCorrectFormat(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure("Email is empty", ErrorType.Validation);

        if (!EmailRegex.IsMatch(email))
            return Result.Failure("Email not in correct VIA format", ErrorType.Validation);

        return Result.Success();
    }

    public override string ToString() => StringValue;
}