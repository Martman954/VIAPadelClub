using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Contracts.Players;

public interface IEmailInUseChecker
{
    bool IsEmailInUse(ViaEmail email);
}