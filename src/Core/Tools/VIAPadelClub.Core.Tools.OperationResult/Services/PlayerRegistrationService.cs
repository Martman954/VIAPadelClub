using System.Text.RegularExpressions;
using VIAPadelClub.Core.Tools.OperationResult.OperationResult.Errors;
using VIAPadelClub.Core.Tools.OperationResult.OperationResult.Errors.Registration;
using VIAPadelClub.Core.Tools.OperationResult.Result;

namespace VIAPadelClub.Core.Tools.OperationResult.Services;

/// <summary>
/// Implements the business logic for registering new players.
/// </summary>
public class PlayerRegistrationService
{
    private static readonly Regex EmailRegex = new (@"^([a-zA-Z]{3,4}|[0-9]{6})$");
    private static readonly Regex NameRegex = new (@"^[a-zA-Z]{2,}$");

    public Result<Player.Player> Register(string? email, string? firstName, string? lastName, string? uri)
    {
        var errors = new List<Error>();
        email = email?.Trim().ToLower();
        
        //Email validation
        if (string.IsNullOrWhiteSpace(email))
        {
            errors.Add(RegistrationErrors.EmailEmpty);
        }
        else if (!email.EndsWith("@via.dk", StringComparison.OrdinalIgnoreCase))
        {
            errors.Add(RegistrationErrors.InvalidDomain);
        }
        else
        {
            var localPart = email.Split('@')[0];
            if (!EmailRegex.IsMatch(localPart))
            {
                errors.Add(RegistrationErrors.InvalidFormat);
            }
        }

        if (email?.ToLower() == "already@via.dk")
        {
            errors.Add(RegistrationErrors.EmailTaken);
        }
        
        //FirstName validation
        if (string.IsNullOrWhiteSpace(firstName) || !NameRegex.IsMatch(firstName))
        {
            errors.Add(RegistrationErrors.InvalidFirstName);
        }

        //LastName validation
        if (string.IsNullOrWhiteSpace(lastName) || !NameRegex.IsMatch(lastName))
        {
            errors.Add(RegistrationErrors.InvalidLastName);
        }

        //UriVlidation validation
        if (string.IsNullOrWhiteSpace(uri))
        {
            errors.Add(RegistrationErrors.InvalidUri);
        }
        
        if (errors.Any())
        {
            return Result<Player.Player>.Failure(errors);
        }
        
        var newPlayer = new Player.Player(
            Id: new Random().Next(1000, 9999),
            Email: email!.ToLower(),
            FirstName: FormatName(firstName!),
            LastName: FormatName(lastName!),
            ProfilePictureUri: uri!
        );

        return newPlayer; 
    }
    
    private string FormatName(string name) => 
        char.ToUpper(name[0]) + name.Substring(1).ToLower();
}