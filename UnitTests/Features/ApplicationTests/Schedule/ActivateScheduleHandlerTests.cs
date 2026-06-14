using VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;
using VIAPadelClub.Core.Application.Features.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Schedules;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;
using ScheduleAggregate = VIAPadelClub.Core.Domain.Aggregates.Schedules.Schedule;

namespace UnitTests.Features.ApplicationTests.Schedule;

file class ActivateFakeScheduleRepo : IScheduleRepository
{
    private List<ScheduleAggregate> Schedules { get; } = [];

    public Task AddAsync(ScheduleAggregate schedule)
    {
        Schedules.Add(schedule);
        return Task.CompletedTask;
    }

    public Task<ScheduleAggregate?> GetAsync(ScheduleId scheduleId)
        => Task.FromResult(Schedules.FirstOrDefault(s => s.Id.Equals(scheduleId)));

    public Task RemoveAsync(ScheduleId scheduleId)
    {
        var schedule = Schedules.FirstOrDefault(s => s.Id.Equals(scheduleId));
        if (schedule != null)
            Schedules.Remove(schedule);
        return Task.CompletedTask;
    }
}

file class ActivateNoConflictChecker : IScheduleDateConflictChecker
{
    public bool ActiveScheduleExistsOnDate(ScheduleId excludeScheduleId, DateOnly date) => false;
}

file class ActivateConflictChecker : IScheduleDateConflictChecker
{
    public bool ActiveScheduleExistsOnDate(ScheduleId excludeScheduleId, DateOnly date) => true;
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
        await repo.AddAsync(schedule);

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
        await repo.AddAsync(schedule);

        var command = ((Result<ActivateScheduleCommand>.Success)
            ActivateScheduleCommand.Create(schedule.Id.ToString())).Value;

        var result = await handler.HandleAsync(command);

        Assert.IsType<Result<None>.Success>(result);
    }
}

