using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Domain.Aggregates.Player.ValueObjects;

public class Quarantine
{
    public TimeInterval TimeInterval { get; }
    
    // Foreign key ???? 
    public ViaEmail ViaEmail { get; }

    private Quarantine(TimeInterval timeInterval, ViaEmail email)
    {
        TimeInterval = timeInterval;
        ViaEmail = email;
    }

    public static Result<Quarantine> Create(TimeInterval timeInterval, ViaEmail email)
    {
        return new Quarantine(timeInterval, email);
    }


}