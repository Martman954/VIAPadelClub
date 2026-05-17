using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Contracts.Player;

public interface IPlayerBookingFinder
{
    IReadOnlyList<Aggregates.Court.Court> GetCourtsWithBookingsForPlayer(ViaEmail email);
}


