using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Contracts.Court;

public interface ICourtHasBookingChecker
{
       bool HasBooking(ViaEmail email, DateTime date);
} 