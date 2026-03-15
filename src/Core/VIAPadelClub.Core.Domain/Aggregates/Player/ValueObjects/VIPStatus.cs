using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Domain.Aggregates.Player.ValueObjects;

public class VipStatus
{
    public TimeInterval TimeInterval { get; }

    private VipStatus(TimeInterval timeInterval)
    {
        TimeInterval = timeInterval;
    }

    public static Result<VipStatus> Create(TimeInterval timeInterval)
    {
        return new VipStatus(timeInterval);
    }
}