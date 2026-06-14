namespace VIAPadelClub.Core.QueryContracts;

public static class ManagerScheduleOverview
{
	public sealed record Query(int Year, int Month);

	public sealed record Answer(
		int Year,
		int Month,
		IReadOnlyList<DayStatusItem> Days);

	public sealed record DayStatusItem(
		DateOnly Date,
		string? Status);
}