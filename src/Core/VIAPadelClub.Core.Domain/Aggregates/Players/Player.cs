using VIAPadelClub.Core.Domain.Aggregates.Players.ValueObjects;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Players;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Aggregates.Players;

public sealed class Player
{
    public ViaEmail Email { get; }
    public Name Name { get; }
    public ImageUrl ProfilePictureUri { get; }
    public VipStatus? VipStatus { get; private set; }
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

    internal Result<None> Blacklist()
    {
        isBlackListed = true;
        VipStatus = null;
        Quarantine = null;
        return Result.Success();
    }

    public Result<None> LiftBlacklist()
    {
        if (!isBlackListed)
            return Result.Failure("Player is not blacklisted.", ErrorType.Validation);

        isBlackListed = false;
        return Result.Success();
    }

    public Result<None> ElevateToVip(DateTime currentDate)
    {
        if (isBlackListed)
            return Result.Failure("Blacklisted players cannot be elevated to VIP.", ErrorType.Validation);

        if (IsQuarantined(currentDate))
            return Result.Failure("Quarantined players cannot be elevated to VIP.", ErrorType.Validation);

        if (VipStatus != null && VipStatus.IsActive(currentDate))
        {
            VipStatus.ExtendByThirtyDays();
        }
        else
        {
            VipStatus = VipStatus.Create(currentDate);
        }

        return Result.Success();
    }
    
}