using System;
using System.Collections.Generic;

namespace VIAPadelClub.Infrastructure.EfcQueries.Scaffolded;

public partial class QueueEntry
{
    public string QueueId { get; set; } = null!;

    public string PlayerId { get; set; } = null!;

    public string ScheduleId { get; set; } = null!;

    public string CourtId { get; set; } = null!;

    public DateTime RequestedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual Court Court { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;

    public virtual Schedule Schedule { get; set; } = null!;
}
