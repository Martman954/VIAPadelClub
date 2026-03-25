using VIAPadelClub.Core.Application.Abstractions;
using VIAPadelClub.Core.Domain.Aggregates.Schedule;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Application.Schedules.Handlers;

public class CreateScheduleHandler : ICommandHandler<CreateScheduleCommand>
{
    public async Task<Result> HandleAsync(CreateScheduleCommand command)
    {
        var result = Schedule.Create(command.Date, command.Intervals);
        
        if (result is Result<Schedule>.Failure failure)
        {
            return Result.Failure<None>(failure.Errors);
        }
        
        Console.WriteLine($"[LOG] Created schedule for {command.Date}");
        return Result.Success();
    }
}