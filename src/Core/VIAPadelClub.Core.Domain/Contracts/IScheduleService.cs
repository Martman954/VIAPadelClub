using VIAPadelClub.Core.Domain.Aggregates.Schedule;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Domain.Contracts;

public interface IScheduleService
{
    Task<Result<Schedules>> GetSchedule(Guid scheduleId);
    Task<Result<Schedules>> GetScheduleByDate(DateOnly scheduleDate);
    Task<bool> ExistsOnDate(DateOnly scheduleDate);
}