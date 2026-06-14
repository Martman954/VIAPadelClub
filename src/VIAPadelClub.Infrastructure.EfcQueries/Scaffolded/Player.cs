using System;
using System.Collections.Generic;

namespace VIAPadelClub.Infrastructure.EfcQueries.Scaffolded;

public partial class Player
{
    public string PlayerId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? ProfileImageUrl { get; set; }

    public DateOnly? VipUntil { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<QueueEntry> QueueEntries { get; set; } = new List<QueueEntry>();
}
