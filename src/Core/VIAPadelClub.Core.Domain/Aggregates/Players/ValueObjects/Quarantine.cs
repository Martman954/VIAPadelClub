namespace VIAPadelClub.Core.Domain.Aggregates.Players.ValueObjects;

public class Quarantine
{
    public DateTime StartDate { get; private set; }
    public DateTime EndDate   { get; private set; }

    private Quarantine(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public static Quarantine Create(DateTime currentDate)
    {
        return new Quarantine(currentDate.Date, currentDate.Date.AddDays(3));
    }

    public void ExtendByThreeDays()
    {
        EndDate = EndDate.AddDays(3);
    }

    public bool IsActive(DateTime currentDate) => currentDate.Date <= EndDate.Date;

    public bool CoversDate(DateTime date) => date.Date >= StartDate.Date && date.Date <= EndDate.Date;
}