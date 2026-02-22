using VIAPadelClub.Core.Tools.OperationResult.Errors;

namespace VIAPadelClub.Core.Tools.OperationResult.OperationResult.Errors.Registration;

/// <summary>
/// Contains a centralized list of all potential errors that can occur during the registration process.
/// </summary>
public class RegistrationErrors
{
    public static Error InvalidDomain => new("Email.Domain", "Only people with a VIA mail (@via.dk) can register.", ErrorType.Validation);
    public static Error InvalidFormat => new("Email.Format", "The email format is incorrect.", ErrorType.Validation);
    public static Error EmailEmpty => new("Email.Empty", "Email cannot be empty.", ErrorType.Validation);
    public static Error EmailTaken => new("Email.Conflict", "The email is already registered.", ErrorType.Conflict);
    public static Error InvalidFirstName => new("FirstName.Invalid", "First name must be 2-25 letters and contain no symbols.", ErrorType.Validation);
    public static Error InvalidLastName => new("LastName.Invalid", "Last name must be 2-25 letters and contain no symbols.", ErrorType.Validation);
    public static Error InvalidUri => new("Uri.Invalid", "Profile picture URI has an incorrect format.", ErrorType.Validation);
}