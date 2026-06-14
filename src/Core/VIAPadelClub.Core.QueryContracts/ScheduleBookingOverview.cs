namespace VIAPadelClub.Core.QueryContracts;

public static class ScheduleBookingOverview
{
    public sealed record Query(DateOnly Date);

    public sealed record Answer(
        DateOnly Date,
        IReadOnlyList<CourtColumn> Courts);

    public sealed record CourtColumn(
        string CourtId,
        string CourtLabel,
        IReadOnlyList<TimeSlotItem> Slots);

    public sealed record TimeSlotItem(
        TimeOnly Start,
        TimeOnly End,
        bool IsVip,
        BookingItem? Booking);

    public sealed record BookingItem(
        string BookingId,
        string PlayerDisplayName);
}