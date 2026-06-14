using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Aggregates.Schedules.Enums;
using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Infrastructure.EfcDomainPersistence;

internal sealed class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasConversion(
                id => id.GuidValue,
                g  => ScheduleId.From(g));

        builder.Property(s => s.Status)
            .HasConversion(
                s => s.ToString(),
                s => Enum.Parse<Status>(s));

        // Times -- backing field: private List<ScheduleTimeInterval> _times
        builder.OwnsMany(s => s.Times, ti =>
        {
            ti.Property<int>("_rowId").ValueGeneratedOnAdd();
            ti.HasKey("_rowId");
            ti.Property(t => t.IsVip);
            ti.OwnsOne(t => t.TimeInterval, iv =>
            {
                iv.Property(t => t.Start).HasColumnName("Start");
                iv.Property(t => t.End).HasColumnName("End");
            });
        });

        builder.Navigation(s => s.Times)
            .HasField("_times")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Courts -- backing field: private List<CourtId> _courts
        builder.OwnsMany(s => s.Courts, courts =>
        {
            courts.Property<int>("_rowId").ValueGeneratedOnAdd();
            courts.HasKey("_rowId");
            courts.Property(c => c.StringValue).HasColumnName("CourtId");
        });

        builder.Navigation(s => s.Courts)
            .HasField("_courts")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(s => s.VipTimes);
        builder.Ignore(s => s.RegularTimes);
    }
}

