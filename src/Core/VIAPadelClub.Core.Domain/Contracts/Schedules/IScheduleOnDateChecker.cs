namespace VIAPadelClub.Core.Domain.Contracts.Schedules;

public interface IScheduleOnDateChecker
{
     // TODO: delete this interface and conflict checker as well,
     // create a GetMany (by date) method in schedule repo instead 
     
     // TODO: mb make repository generic with public TEntity Find(params object[] keys) method
     // inspo : https://brianbu.com/2019/09/25/the-repository-pattern-isnt-an-anti-pattern-youre-just-doing-it-wrong/
     bool HasScheduleOnDate(DateOnly scheduleDate);
}