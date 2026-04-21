using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Contracts.Player;

public interface IEmailInUseChecker
{
    bool IsEmailInUse(ViaEmail email);
}