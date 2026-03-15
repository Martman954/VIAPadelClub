using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Aggregates.Court.Entities;

public class Booking
{
    public BookingId Id { get; }
    public TimeInterval TimeInterval { get; }
    public Guid ScheduleId { get; }
    public ViaEmail PlayerEmail { get; }
    public bool IsCancelled { get; private set; }

    internal Booking(BookingId id, TimeInterval timeInterval, Guid scheduleId, ViaEmail playerEmail)
    {
        Id = id;
        TimeInterval = timeInterval;
        ScheduleId = scheduleId;
        PlayerEmail = playerEmail;
        IsCancelled = false;
    }

    internal void Cancel()
    {
        IsCancelled = true;
    }
}