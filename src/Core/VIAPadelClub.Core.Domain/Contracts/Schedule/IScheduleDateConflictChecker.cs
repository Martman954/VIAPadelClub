namespace VIAPadelClub.Core.Domain.Contracts.Schedule;

public interface IScheduleDateConflictChecker
{
    bool ActiveScheduleExistsOnDate(Guid excludeScheduleId, DateOnly date);
}

