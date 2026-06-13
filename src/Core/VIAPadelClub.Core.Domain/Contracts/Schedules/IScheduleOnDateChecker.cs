namespace VIAPadelClub.Core.Domain.Contracts.Schedules;

public interface IScheduleOnDateChecker
{
     bool HasScheduleOnDate(DateOnly scheduleDate);
}