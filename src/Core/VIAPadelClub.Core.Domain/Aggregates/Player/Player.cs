using VIAPadelClub.Core.Domain.Aggregates.Player.ValueObjects;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Player;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Aggregates.Player;

public sealed class Player
{
    public ViaEmail Email { get; }
    public Name Name { get; }
    public ImageUrl ProfilePictureUri { get; }
    public VipStatus? VipStatus { get; }
    public Quarantine? Quarantine { get; private set; }
    public bool isBlackListed { get; private set; }

    private Player(ViaEmail email, Name name, ImageUrl profilePictureUri)
    {
        Email = email;
        Name = name;
        ProfilePictureUri = profilePictureUri;
        VipStatus = null;
        Quarantine = null;
        isBlackListed = false;
    }

    public static Result<Player> Register(
        ViaEmail email, Name fullName, ImageUrl profilePictureUri,
        IEmailInUseChecker emailInUseChecker)
    {
        if (emailInUseChecker.IsEmailInUse(email))
            return Result.Failure<Player>(new ResultError("Email in use", ErrorType.Conflict));
        
        return new Player(email,fullName, profilePictureUri);
    }
    
    internal Result<None> QuarantinePlayer(DateTime currentDate)
    {
        if (Quarantine != null && Quarantine.IsActive(currentDate))
        {
            Quarantine.ExtendByThreeDays();
        }
        else
        {
            Quarantine = Quarantine.Create(currentDate);
        }

        return Result.Success();
    }

    public bool IsQuarantined(DateTime currentDate) =>
        Quarantine != null && Quarantine.IsActive(currentDate);

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