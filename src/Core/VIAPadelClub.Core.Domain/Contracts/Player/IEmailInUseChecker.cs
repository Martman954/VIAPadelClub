using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Domain.Contracts;

public interface IEmailInUseChecker
{
    bool IsEmailInUse(ViaEmail email);
}