using VIAPadelClub.Core.Domain.Aggregates.Schedule;
using VIAPadelClub.Core.Domain.Aggregates.Schedule.ValueObjects;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Domain.Contracts;

public interface IScheduleService
{
    Task<Result<Schedule>> GetSchedule(Guid scheduleId);
    Task<Result<Schedule>> GetScheduleByDate(DateOnly scheduleDate);
}