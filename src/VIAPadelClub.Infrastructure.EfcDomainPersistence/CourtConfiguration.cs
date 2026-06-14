using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VIAPadelClub.Core.Domain.Aggregates.Courts;
using VIAPadelClub.Core.Domain.Aggregates.Courts.Entities;
using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Infrastructure.EfcDomainPersistence;

internal sealed class CourtConfiguration : IEntityTypeConfiguration<Court>
{
    public void Configure(EntityTypeBuilder<Court> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(
                id => id.StringValue,
                s  => CourtId.From(s));

        // Bookings -- backing field: private readonly List<Booking> _bookings
        builder.OwnsMany(c => c.Bookings, booking =>
        {
            booking.Property(b => b.Id)
                .HasConversion(
                    id => id.Value,
                    g  => BookingId.From(g));

            booking.HasKey(b => b.Id);

            booking.OwnsOne(b => b.TimeInterval, ti =>
            {
                ti.Property(t => t.Start).HasColumnName("Start");
                ti.Property(t => t.End).HasColumnName("End");
            });

            booking.Property(b => b.ScheduleId);

            booking.Property(b => b.PlayerEmail)
                .HasConversion(
                    email => email.StringValue,
                    s     => ViaEmail.From(s));

            booking.Property(b => b.IsCancelled);
        });

        builder.Navigation(c => c.Bookings)
            .HasField("_bookings")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}


