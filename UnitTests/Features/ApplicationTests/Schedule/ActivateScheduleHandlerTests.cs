using VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;
using VIAPadelClub.Core.Application.Features.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Schedules;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;
using ScheduleAggregate = VIAPadelClub.Core.Domain.Aggregates.Schedules.Schedule;

namespace UnitTests.Features.ApplicationTests.Schedule;

file class ActivateFakeScheduleRepo : IScheduleRepo
{
    private List<ScheduleAggregate> Schedules { get; } = [];

    public Task<ScheduleAggregate> AddSchedule(ScheduleAggregate schedule)
    {
        Schedules.Add(schedule);
        return Task.FromResult(schedule);
    }

    public Task<ScheduleAggregate> GetSchedule(Guid scheduleId)
        => Task.FromResult(Schedules.First(s => s.Id == scheduleId));

    public Task<ScheduleAggregate> RemoveSchedule(Guid scheduleId)
    {
        var schedule = Schedules.First(s => s.Id == scheduleId);
        Schedules.Remove(schedule);
        return Task.FromResult(schedule);
    }
}

file class ActivateNoConflictChecker : IScheduleDateConflictChecker
{
    public bool ActiveScheduleExistsOnDate(Guid excludeScheduleId, DateOnly date) => false;
}

file class ActivateConflictChecker : IScheduleDateConflictChecker
{
    public bool ActiveScheduleExistsOnDate(Guid excludeScheduleId, DateOnly date) => true;
}

public class ActivateScheduleHandlerTests
{
    [Fact]
    public void CreateCommand_InvalidScheduleId_ReturnsFailure()
    {
        var result = ActivateScheduleCommand.Create("not-a-guid");

        Assert.IsType<Result<ActivateScheduleCommand>.Failure>(result);
    }

    [Fact]
    public async Task HandleAsync_ScheduleNotFound_ReturnsNotFoundFailure()
    {
        var repo = new ActivateFakeScheduleRepo();
        var checker = new ActivateNoConflictChecker();
        var handler = new ActivateScheduleHandler(repo, checker);
        var command = ((Result<ActivateScheduleCommand>.Success)
            ActivateScheduleCommand.Create(Guid.NewGuid().ToString())).Value;

        var result = await handler.HandleAsync(command);

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.ErrorType == ErrorType.NotFound);
    }

    [Fact]
    public async Task HandleAsync_DateConflict_ReturnsFailureAndDoesNotSave()
    {
        var repo = new ActivateFakeScheduleRepo();
        var checker = new ActivateConflictChecker();
        var handler = new ActivateScheduleHandler(repo, checker);

        var schedule = ScheduleAggregate.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        schedule.AddCourt(CourtId.Create("S1").Payload);
        await repo.AddSchedule(schedule);

        var command = ((Result<ActivateScheduleCommand>.Success)
            ActivateScheduleCommand.Create(schedule.Id.ToString())).Value;

        var result = await handler.HandleAsync(command);

        Assert.IsType<Result<None>.Failure>(result);
    }

    [Fact]
    public async Task HandleAsync_ValidSchedule_Activates()
    {
        var repo = new ActivateFakeScheduleRepo();
        var checker = new ActivateNoConflictChecker();
        var handler = new ActivateScheduleHandler(repo, checker);

        var schedule = ScheduleAggregate.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        schedule.AddCourt(CourtId.Create("S1").Payload);
        await repo.AddSchedule(schedule);

        var command = ((Result<ActivateScheduleCommand>.Success)
            ActivateScheduleCommand.Create(schedule.Id.ToString())).Value;

        var result = await handler.HandleAsync(command);

        Assert.IsType<Result<None>.Success>(result);
    }
}

