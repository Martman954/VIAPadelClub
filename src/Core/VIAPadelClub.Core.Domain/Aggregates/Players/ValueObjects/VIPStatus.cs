namespace VIAPadelClub.Core.Domain.Aggregates.Players.ValueObjects;

public class VipStatus
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; private set; }

    private VipStatus(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public static VipStatus Create(DateTime currentDate)
    {
        return new VipStatus(currentDate.Date, currentDate.Date.AddMonths(1));
    }

    public void ExtendByThirtyDays()
    {
        EndDate = EndDate.AddDays(30);
    }

    public bool IsActive(DateTime currentDate) => currentDate.Date <= EndDate.Date;
}