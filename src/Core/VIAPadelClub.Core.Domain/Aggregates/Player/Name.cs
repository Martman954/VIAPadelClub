using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Aggregates.Player;

public class Name
{
    public string FirstName { get; }
    public string LastName { get; }
    
    private Name(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
    
    public static Result<Name> CreateName(
        string firstname, string lastname)
        =>
            Result.Combine(
                NameMustBeCorrectFormat(firstname),
                NameMustBeCorrectFormat(lastname)
            ).WithSuccessPayload(new Name(firstname, lastname));
        
    
        
    

    
    //  Can add more validation
    private static Result<None> NameMustBeCorrectFormat(string name) =>
        (string.IsNullOrWhiteSpace(name))
            ? Result.Failure("Name not in correct format", ErrorType.Validation)
            : Result.Success();

}