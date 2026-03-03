using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Aggregates.Player;

public class ViaEmail
{
    public string Value { get; }

    private ViaEmail(string email)
    {
        Value = email;
    }
    
    public static Result<ViaEmail> CreateEmail(string email)
        =>
            Result.Combine(
                EmailMustBeCorrectFormat(email)
            ).WithSuccessPayload(new ViaEmail(email));


    
    private static Result<None> EmailMustBeCorrectFormat(string email) =>
        (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            ? Result.Failure("Email not in correct format", ErrorType.Validation)
            : Result.Success();

}