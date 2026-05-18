using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Contracts.Schedules;

public interface INonVipBookingOverlapChecker
{
    bool HasNonVipBookingsInTimeSpan(Guid scheduleId, TimeInterval timeInterval);
}

