using VIAPadelClub.Core.Domain.Aggregates.Schedule.ValueObjects;

namespace VIAPadelClub.Core.Application.Schedules;

public record CreateScheduleCommand(
    DateTime Date, 
    List<ScheduleTimeInterval> Intervals);