using System;
using System.Collections.Generic;

namespace VIAPadelClub.Infrastructure.EfcQueries.Scaffolded;

public partial class Schedule
{
    public string ScheduleId { get; set; } = null!;

    public DateOnly ScheduleDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<QueueEntry> QueueEntries { get; set; } = new List<QueueEntry>();

    public virtual ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();

    public virtual ICollection<Court> Courts { get; set; } = new List<Court>();
}
