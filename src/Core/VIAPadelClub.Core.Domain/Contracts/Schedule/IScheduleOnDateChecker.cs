namespace VIAPadelClub.Core.Domain.Contracts.Schedule;

public interface IScheduleOnDateChecker
{
     bool HasScheduleOnDate(DateOnly scheduleDate);
}