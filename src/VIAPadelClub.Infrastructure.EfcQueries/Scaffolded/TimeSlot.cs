using System;
using System.Collections.Generic;

namespace VIAPadelClub.Infrastructure.EfcQueries.Scaffolded;

public partial class TimeSlot
{
    public int TimeSlotId { get; set; }

    public string ScheduleId { get; set; } = null!;

    public string CourtId { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int IsVip { get; set; }

    public virtual Court Court { get; set; } = null!;

    public virtual Schedule Schedule { get; set; } = null!;
}
