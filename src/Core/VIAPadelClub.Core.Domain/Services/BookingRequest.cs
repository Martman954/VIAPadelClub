using VIAPadelClub.Core.Domain.Aggregates.Court;
using VIAPadelClub.Core.Domain.Aggregates.Player;
using VIAPadelClub.Core.Domain.Aggregates.Schedule;
using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Services;

public record BookingRequest(Player Player, Court Court, Schedule Schedule, TimeInterval TimeInterval);

