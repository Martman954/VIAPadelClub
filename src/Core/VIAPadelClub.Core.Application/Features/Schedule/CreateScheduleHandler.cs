using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Domain.Common.ScheduleCommands;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.Features.Schedule;

internal class CreateScheduleHandler : ICommandHandler<CreateScheduleCommand>
{
    // TODO: 
    // Some schedules repo
    // Some unit of work (related to transactions)
    
    public Task<Result> HandleAsync(CreateScheduleCommand command)
    {
        // Transaction script here!
        throw new NotImplementedException();
    }
}