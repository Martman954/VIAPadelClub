using System.Text.RegularExpressions;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Aggregates.Player.ValueObjects;

public class Name
{
    public string FirstName { get; }
    public string LastName { get; }

    private static readonly Regex NameRegex =
        new(@"^[a-zA-Z]{2,25}$", RegexOptions.Compiled);

    private Name(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static Result<Name> CreateName(string firstname, string lastname)
        =>
            Result.Combine(
                NameMustBeCorrectFormat(firstname),
                NameMustBeCorrectFormat(lastname)
            ).WithSuccessPayload(
                new Name(
                    Normalize(firstname),
                    Normalize(lastname)
                )
            );

    private static Result<None> NameMustBeCorrectFormat(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Name is empty", ErrorType.Conflict);

        if (!NameRegex.IsMatch(name))
            return Result.Failure("Name must be 2–25 letters only", ErrorType.ImATeaPot);

        return Result.Success();
    }

    private static string Normalize(string name)
    {
        return char.ToUpper(name[0]) + name.Substring(1).ToLower();
    }
}