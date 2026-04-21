using VIAPadelClub.Core.Domain.Aggregates.Schedule;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Domain.Contracts;

public interface IScheduleService
{
    Task<Result<Aggregates.Schedule.Schedule>> GetSchedule(Guid scheduleId);
    Task<Result<Aggregates.Schedule.Schedule>> GetScheduleByDate(DateOnly scheduleDate);
}