using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Contracts.Schedules;

public interface IScheduleDateConflictChecker
{
   // TODO: delete this, create a GetMany (by date) method in schedule repo instead
    bool ActiveScheduleExistsOnDate(ScheduleId excludeScheduleId, DateOnly date);
}

