using VIAPadelClub.Core.Domain.Aggregates.Court.Entities;
using VIAPadelClub.Core.Domain.Aggregates.Player;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Domain.Contracts;

public interface IPlayerService
{
    Task<Result<Player>> GetPlayer(ViaEmail viaEmail);
    Task<Result<List<Booking>>> GetPlayerBookingsByDate(Guid playerId);
}