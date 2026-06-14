using VIAPadelClub.Core.Domain.Aggregates.Courts;

namespace VIAPadelClub.Core.Domain.Contracts.Courts;

public interface IBookingCourtFinder
{
    Task<Court?> FindCourtWithBooking(Common.Values.BookingId bookingId);
}

