using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Contracts.Schedule;

public interface INonVipBookingOverlapChecker
{
    bool HasNonVipBookingsInTimeSpan(Guid scheduleId, TimeInterval timeInterval);
}

