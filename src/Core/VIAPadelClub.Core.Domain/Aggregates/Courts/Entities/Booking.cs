using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Aggregates.Courts.Entities;

public class Booking
{
    public BookingId Id { get; private set; } = null!;
    public TimeInterval TimeInterval { get; private set; } = null!;
    public Guid ScheduleId { get; private set; }
    public ViaEmail PlayerEmail { get; private set; } = null!;
    public bool IsCancelled { get; private set; }

    internal Booking(BookingId id, TimeInterval timeInterval, Guid scheduleId, ViaEmail playerEmail)
    {
        Id = id;
        TimeInterval = timeInterval;
        ScheduleId = scheduleId;
        PlayerEmail = playerEmail;
        IsCancelled = false;
    }

    /// <summary>For EF Core use only.</summary>
    private Booking() { }

    internal void Cancel()
    {
        IsCancelled = true;
    }
}