using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Contracts.Courts;

public interface ICourtHasBookingChecker
{
       bool HasBooking(ViaEmail email, DateTime date);
} 