using VIAPadelClub.Core.Domain.Common.Bases;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Aggregates.Player;


// Class just to test functionality of Result pattern
public sealed class Player
{
    public ViaEmail Email { get; }
    public Name Name { get; }
    public ImageUrl ProfilePictureUri { get; }
    public VipStatus VipStatus { get; set; }

//    public Quarantine Quarantine { get; }
    public bool isBlackListed { get; set; }
    //List<BookingId> bookings

    private Player(ViaEmail email, Name name, ImageUrl profilePictureUri)
    {
        Email = email;
        Name = name;
        ProfilePictureUri = profilePictureUri;
        VipStatus = null;
        isBlackListed = false;        
    }

    public static Result<Player> Register(
        ViaEmail email, Name fullName, ImageUrl profilePictureUri)
    {
        return new Player(email, fullName, profilePictureUri);
    }

    public Result<Quarantine> QuarantinePlayer(TimeInterval timeInterval, ViaEmail email)
    {
        return Quarantine.Create(timeInterval, email);
    }

    public Result<None> Blacklist()
    {
        isBlackListed = true;
        return Result.Success();
    }
        
    public Result<None> LiftBlacklist()
    {
        isBlackListed = false;
        return Result.Success();
    }

    public Result<None> ElevateToVip(TimeInterval timeInterval)
    {
        return VipStatus.Create(timeInterval) switch
        {
            Result<VipStatus>.Success => Result.Success(),
            Result<VipStatus>.Failure f => Result.Failure<None>(f.Errors),
            _ => throw new InvalidOperationException()
        };
    }
}