using VIAPadelClub.Core.Domain.Aggregates.Courts;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Core.Domain.Services;

public record BookingRequest(Player Player, Court Court, Schedule Schedule, TimeInterval TimeInterval);

