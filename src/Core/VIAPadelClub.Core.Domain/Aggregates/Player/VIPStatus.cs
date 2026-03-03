using VIAPadelClub.Core.Domain.Common.Bases;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Domain.Aggregates.Player;

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