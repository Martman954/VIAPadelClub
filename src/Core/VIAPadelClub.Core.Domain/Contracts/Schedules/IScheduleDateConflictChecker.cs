namespace VIAPadelClub.Core.Domain.Contracts.Schedules;

public interface IScheduleDateConflictChecker
{
    bool ActiveScheduleExistsOnDate(Guid excludeScheduleId, DateOnly date);
}

