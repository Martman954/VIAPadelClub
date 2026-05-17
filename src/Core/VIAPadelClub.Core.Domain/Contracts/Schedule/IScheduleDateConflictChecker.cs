namespace VIAPadelClub.Core.Domain.Contracts.Schedule;

public interface IScheduleDateConflictChecker
{
   // TODO: delete this, create a GetMany (by date) method in schedule repo instead
    bool ActiveScheduleExistsOnDate(Guid excludeScheduleId, DateOnly date);
}

