using VIAPadelClub.Core.Domain.Aggregates.Courts;
using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Contracts.Players;

public interface IPlayerBookingFinder
{
    IReadOnlyList<Court> GetCourtsWithBookingsForPlayer(ViaEmail email);
}


