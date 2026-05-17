namespace VIAPadelClub.Core.Domain.Aggregates.Player.ValueObjects;

public class Quarantine
{
    private DateTime StartDate { get; }
    private DateTime EndDate { get; set; }

    private Quarantine(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public static Quarantine Create(DateTime currentDate)
    {
        var start = currentDate.Date;
        var end = currentDate.Date.AddDays(3).AddHours(23).AddMinutes(59).AddSeconds(59);
        return new Quarantine(start, end);
    }

    public void ExtendByThreeDays()
    {
        EndDate = EndDate.Date.AddDays(3).AddHours(23).AddMinutes(59).AddSeconds(59);
    }

    public bool IsActive(DateTime currentDate) => currentDate <= EndDate;

    public bool CoversDate(DateTime date) => date.Date >= StartDate.Date && date.Date <= EndDate.Date;
}