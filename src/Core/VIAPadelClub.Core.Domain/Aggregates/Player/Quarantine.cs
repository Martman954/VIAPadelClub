using VIAPadelClub.Core.Domain.Common.Bases;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Domain.Aggregates.Player;

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