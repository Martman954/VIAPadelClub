using System;
using System.Collections.Generic;

namespace VIAPadelClub.Infrastructure.EfcQueries.Scaffolded;

public partial class Booking
{
    public string BookingId { get; set; } = null!;

    public string ScheduleId { get; set; } = null!;

    public string CourtId { get; set; } = null!;

    public string PlayerId { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int IsCancelled { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Court Court { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;

    public virtual Schedule Schedule { get; set; } = null!;
}
