using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Players;

namespace UnitTests.Fakes;

internal class FakeEmailChecker : IEmailInUseChecker
{
    public bool IsEmailInUse(ViaEmail email) => false;
}

internal class EmailInUseChecker : IEmailInUseChecker
{
    public bool IsEmailInUse(ViaEmail email) => true;
}