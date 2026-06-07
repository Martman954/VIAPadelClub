using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Schedules;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.ScheduleTests;

file class NoConflictChecker : IScheduleDateConflictChecker
{
    public bool ActiveScheduleExistsOnDate(Guid excludeScheduleId, DateOnly date) => false;
}

file class NoOverlapChecker : INonVipBookingOverlapChecker
{
    public bool HasNonVipBookingsInTimeSpan(Guid scheduleId, TimeInterval timeInterval) => false;
}

file class HasOverlapChecker : INonVipBookingOverlapChecker
{
    public bool HasNonVipBookingsInTimeSpan(Guid scheduleId, TimeInterval timeInterval) => true;
}

public class ScheduleMarkVipTimeSpanTests
{
    private static TimeInterval ValidVipInterval()
    {
        var date = DateTime.Today.AddDays(1);
        return TimeInterval.Create(date.AddHours(16), date.AddHours(17)).Payload;
    }

    [Fact]
    public void MarkVip_OnDraftSchedule_ValidInterval_ReturnsSuccess()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));

        var result = schedule.MarkVipTimeSpan(ValidVipInterval(), new NoOverlapChecker());

        Assert.IsType<Result<None>.Success>(result);
    }

    [Fact]
    public void MarkVip_OnDraftSchedule_VipSlotAppearsInVipTimes()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        schedule.MarkVipTimeSpan(ValidVipInterval(), new NoOverlapChecker());

        Assert.NotEmpty(schedule.VipTimes);
    }

    [Fact]
    public void MarkVip_OnDraftSchedule_RegularTimesStillExist()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        schedule.MarkVipTimeSpan(ValidVipInterval(), new NoOverlapChecker());

        Assert.NotEmpty(schedule.RegularTimes);
    }

    [Fact]
    public void MarkVip_OnActiveSchedule_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;
        var targetDate = DateTime.Today.AddDays(1);
        schedule.UpdateDate(targetDate);
        
        schedule.AddCourt(CourtId.Create("S1").Payload);
        schedule.Activate(new NoConflictChecker());
        
        var safeInterval = TimeInterval.Create(targetDate.AddHours(12), targetDate.AddHours(13)).Payload;

        var result = schedule.MarkVipTimeSpan(safeInterval, new NoOverlapChecker());

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("Draft", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void MarkVip_OnDeletedSchedule_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        typeof(Schedule)
            .GetMethod("Delete", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .Invoke(schedule, [DateTime.Today]);

        var result = schedule.MarkVipTimeSpan(ValidVipInterval(), new NoOverlapChecker());

        Assert.IsType<Result<None>.Failure>(result);
    }

    [Fact]
    public void MarkVip_IntervalLessThan30Minutes_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        var date = DateTime.Today.AddDays(1);
        var shortInterval = TimeInterval.Create(date.AddHours(15), date.AddHours(15).AddMinutes(29)).Payload;

        var result = schedule.MarkVipTimeSpan(shortInterval, new NoOverlapChecker());

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("30 minutes", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void MarkVip_StartNotOnHourOrHalfHour_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        var date = DateTime.Today.AddDays(1);
        var badFormat = TimeInterval.Create(date.AddHours(15).AddMinutes(15), date.AddHours(16).AddMinutes(15)).Payload;

        var result = schedule.MarkVipTimeSpan(badFormat, new NoOverlapChecker());

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e =>
            e.Message.Contains(":00", StringComparison.OrdinalIgnoreCase) ||
            e.Message.Contains(":30", StringComparison.OrdinalIgnoreCase) ||
            e.Message.Contains("half hour", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void MarkVip_OutsideScheduleBounds_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        var date = DateTime.Today.AddDays(1);
        var outOfBounds = TimeInterval.Create(date.AddHours(21), date.AddHours(23)).Payload;

        var result = schedule.MarkVipTimeSpan(outOfBounds, new NoOverlapChecker());

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("boundar", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void MarkVip_OverlapsNonVipBooking_ReturnsFailure()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));

        var result = schedule.MarkVipTimeSpan(ValidVipInterval(), new HasOverlapChecker());

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("booking", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void MarkVip_TwoNonOverlappingIntervals_BothAppearInVipTimes()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        var date = DateTime.Today.AddDays(1);
        var first  = TimeInterval.Create(date.AddHours(16), date.AddHours(17)).Payload;
        var second = TimeInterval.Create(date.AddHours(18), date.AddHours(19)).Payload;

        schedule.MarkVipTimeSpan(first,  new NoOverlapChecker());
        schedule.MarkVipTimeSpan(second, new NoOverlapChecker());

        Assert.Equal(2, schedule.VipTimes.Count);
    }

    [Fact]
    public void MarkVip_TwoOverlappingIntervals_AreMergedIntoOne()
    {
        var schedule = Schedule.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        var date = DateTime.Today.AddDays(1);
        var first  = TimeInterval.Create(date.AddHours(16), date.AddHours(17).AddMinutes(30)).Payload;
        var second = TimeInterval.Create(date.AddHours(17), date.AddHours(18)).Payload;

        schedule.MarkVipTimeSpan(first,  new NoOverlapChecker());
        schedule.MarkVipTimeSpan(second, new NoOverlapChecker());

        Assert.Single(schedule.VipTimes);
    }
}