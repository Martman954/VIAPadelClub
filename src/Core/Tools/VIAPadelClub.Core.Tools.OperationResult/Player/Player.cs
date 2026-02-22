namespace VIAPadelClub.Core.Tools.OperationResult.Player;

/// <summary>
/// Represents a registered Player in the system.
/// </summary>
/// <param name="Id">A unique identifier generated upon successful registration.</param>
/// <param name="Email">The player's VIA email address (stored in all lower-case).</param>
/// <param name="FirstName">The player's first name (formatted with the first letter capitalized).</param>
/// <param name="LastName">The player's last name (formatted with the first letter capitalized).</param>
/// <param name="ProfilePictureUri">A URI pointing to the player's profile picture.</param>
public record Player(
    int Id, 
    string Email, 
    string FirstName, 
    string LastName, 
    string ProfilePictureUri
);