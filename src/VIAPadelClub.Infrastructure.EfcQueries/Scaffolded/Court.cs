using System;
using System.Collections.Generic;

namespace VIAPadelClub.Infrastructure.EfcQueries.Scaffolded;

public partial class Court
{
    public string CourtId { get; set; } = null!;

    public string CourtLabel { get; set; } = null!;

    public string CourtType { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<QueueEntry> QueueEntries { get; set; } = new List<QueueEntry>();

    public virtual ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
