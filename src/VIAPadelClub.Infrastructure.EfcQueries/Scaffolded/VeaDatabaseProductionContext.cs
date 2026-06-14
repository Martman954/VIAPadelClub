using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace VIAPadelClub.Infrastructure.EfcQueries.Scaffolded;

public partial class VeaDatabaseProductionContext : DbContext
{
    public VeaDatabaseProductionContext()
    {
    }

    public VeaDatabaseProductionContext(DbContextOptions<VeaDatabaseProductionContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Court> Courts { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<QueueEntry> QueueEntries { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<TimeSlot> TimeSlots { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return;

        var basePath = AppContext.BaseDirectory;
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("QueryReadDb");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            var fallbackDbPath = Path.Combine(basePath, "VIAPadelClub.Queries.db");
            connectionString = $"Data Source={fallbackDbPath}";
        }

        optionsBuilder.UseSqlite(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasIndex(e => new { e.PlayerId, e.StartTime }, "IX_Bookings_PlayerTime");

            entity.HasIndex(e => new { e.ScheduleId, e.CourtId, e.StartTime }, "IX_Bookings_ScheduleCourtTime");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Court).WithMany(p => p.Bookings).HasForeignKey(d => d.CourtId);

            entity.HasOne(d => d.Player).WithMany(p => p.Bookings).HasForeignKey(d => d.PlayerId);

            entity.HasOne(d => d.Schedule).WithMany(p => p.Bookings).HasForeignKey(d => d.ScheduleId);
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasIndex(e => e.Email, "IX_Players_Email").IsUnique();
        });

        modelBuilder.Entity<QueueEntry>(entity =>
        {
            entity.HasKey(e => e.QueueId);

            entity.HasIndex(e => new { e.PlayerId, e.Status }, "IX_QueueEntries_PlayerStatus");

            entity.HasOne(d => d.Court).WithMany(p => p.QueueEntries).HasForeignKey(d => d.CourtId);

            entity.HasOne(d => d.Player).WithMany(p => p.QueueEntries).HasForeignKey(d => d.PlayerId);

            entity.HasOne(d => d.Schedule).WithMany(p => p.QueueEntries).HasForeignKey(d => d.ScheduleId);
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasIndex(e => e.ScheduleDate, "IX_Schedules_ScheduleDate").IsUnique();

            entity.HasIndex(e => e.ScheduleDate, "IX_Schedules_ScheduleDate");

            entity.HasMany(d => d.Courts).WithMany(p => p.Schedules)
                .UsingEntity<Dictionary<string, object>>(
                    "ScheduleCourt",
                    r => r.HasOne<Court>().WithMany().HasForeignKey("CourtId"),
                    l => l.HasOne<Schedule>().WithMany().HasForeignKey("ScheduleId"),
                    j =>
                    {
                        j.HasKey("ScheduleId", "CourtId");
                        j.ToTable("ScheduleCourts");
                    });
        });

        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasIndex(e => new { e.ScheduleId, e.CourtId, e.StartTime }, "IX_TimeSlots_ScheduleCourtTime");

            entity.HasOne(d => d.Court).WithMany(p => p.TimeSlots).HasForeignKey(d => d.CourtId);

            entity.HasOne(d => d.Schedule).WithMany(p => p.TimeSlots).HasForeignKey(d => d.ScheduleId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
