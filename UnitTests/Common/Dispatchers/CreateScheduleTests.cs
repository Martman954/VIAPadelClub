using VIAPadelClub.Core.Application.Schedules;
using VIAPadelClub.Core.Application.Schedules.Handlers;
using VIAPadelClub.Core.Domain.Aggregates.Schedule.ValueObjects;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Common.Dispatchers;

public class CreateScheduleTests
{
    [Fact]
    public async Task Dispatcher_Should_Execute_CreateScheduleHandler()
    {
        // Arrange
        var handler = new CreateScheduleHandler();
        
        var dispatcher = new VIAPadelClub.Core.Application.Dispatcher(new LocalProvider(handler));
    
        var intervals = new List<ScheduleTimeInterval> 
        { 
            new ScheduleTimeInterval(new TimeInterval(DateTime.Now, DateTime.Now.AddHours(1)), false) 
        };
        var command = new CreateScheduleCommand(DateTime.Now.AddDays(1), intervals);
    
        // Act
        var result = await dispatcher.DispatchAsync(command);
    
        // Assert
        Assert.IsType<Result<None>.Success>(result);
    }
    
    private class LocalProvider(object handler) : IServiceProvider
    {
        public object? GetService(Type serviceType) => handler;
    }
}