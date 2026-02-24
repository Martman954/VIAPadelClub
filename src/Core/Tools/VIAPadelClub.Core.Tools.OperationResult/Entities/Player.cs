using System.Text.RegularExpressions;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Tools.OperationResult.Entities;


// Class just to test functionality of Result pattern
public sealed class Player
{
    public Guid Id { get; }
    public string Firstname { get; }
    public string Lastname { get; }
    public string Email { get; }
    public string ProfilePictureUri { get; }
    

    private Player(Guid id, string firstname, string lastname, string email, string profilePictureUri)
    {
        Id = id;
        Firstname = firstname;
        Lastname = lastname;
        Email = email;
        ProfilePictureUri = profilePictureUri;
    }

    public static Result<Player> Register(
        string firstname, string lastname, string email, string profilePictureUri)
    =>
        Result.Combine(
            NameMustBeCorrectFormat(firstname),
            NameMustBeCorrectFormat(lastname),
            EmailMustBeCorrectFormat(email)
        ).WithSuccessPayload(new Player(Guid.NewGuid(), firstname, lastname, email, profilePictureUri));
        
    
        
    

    
    //  Can add more validation
    private static Result<None> NameMustBeCorrectFormat(string name) =>
        (string.IsNullOrWhiteSpace(name))
            ? Result.Failure("Name not in correct format", ErrorType.Validation)
            : Result.Success();

    private static Result<None> EmailMustBeCorrectFormat(string email) =>
        (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
    ? Result.Failure("Email not in correct format", ErrorType.Validation)
    : Result.Success();

}