namespace VIAPadelClub.Core.QueryContracts.Queries;

public static class ManagerScheduleOverview
{
	public sealed record Query(int Year, int Month) : IQuery<Answer>;

	public sealed record Answer(
		int Year,
		int Month,
		IReadOnlyList<DayStatusItem> Days);

	public sealed record DayStatusItem(
		DateOnly Date,
		string? Status);
}