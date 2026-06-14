namespace VIAPadelClub.Core.QueryContracts;

public static class PlayerProfileOverview
{
    public sealed record Query(string PlayerIdOrEmail);

    public sealed record Answer(
        string PlayerId,
        string FullName,
        string Email,
        string? VipUntilText,
        int UpcomingBookingCount,
        IReadOnlyList<BookingCard> UpcomingBookings,
        IReadOnlyList<BookingCard> PastBookings);

    public sealed record BookingCard(
        string BookingId,
        DateOnly Date,
        TimeOnly Start,
        TimeOnly End,
        string CourtId);
}