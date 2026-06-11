using VIAPadelClub.Core.Domain.Aggregates.Courts;

namespace VIAPadelClub.Core.Domain.Contracts.Courts;

public interface IBookingCourtFinder
{
    /// <summary>
    /// Finds the Court that contains the booking with the given ID.
    /// </summary>
    /// <returns>The Court aggregate containing the booking, or null if not found</returns>
    Task<Court?> FindCourtWithBooking(Common.Values.BookingId bookingId);
}

