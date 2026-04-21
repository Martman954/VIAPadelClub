using VIAPadelClub.Core.Domain.Aggregates.Court;
using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Contracts;

public interface ICourtService
{
    Task<Courts> GetCourtById(Guid courtId);
    Task<bool> HasBookingOnDate(ViaEmail email, DateTime date);
}