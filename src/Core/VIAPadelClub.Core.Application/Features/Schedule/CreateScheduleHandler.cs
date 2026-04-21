using Features.CommandDispatch;
using Features.CommandDispatch.ScheduleCommands;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.Features.Schedule;

internal class CreateScheduleHandler : ICommandHandler<CreateScheduleCommand>
{
    // TODO: 
    // Some repo
    // Some unit of work (related to transactions
    
    public Task<Result> HandleAsync(CreateScheduleCommand command)
    {
        // Transaction script here!
        // Also do the stuff here
        throw new NotImplementedException();
    }
}