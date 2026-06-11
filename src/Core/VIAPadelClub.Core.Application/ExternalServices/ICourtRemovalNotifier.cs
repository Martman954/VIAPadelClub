using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Application.ExternalServices;

public interface ICourtRemovalNotifier
{
    Task NotifyCourtRemovedAsync(IReadOnlyList<ViaEmail> affectedEmails, Guid scheduleId, CourtId courtId);
}

