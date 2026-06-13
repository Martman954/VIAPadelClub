using VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;
using VIAPadelClub.Core.Application.Features.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;
using ScheduleAggregate = VIAPadelClub.Core.Domain.Aggregates.Schedules.Schedule;

namespace UnitTests.Features.ApplicationTests.Schedule;

file class AddCourtFakeScheduleRepo : IScheduleRepo
{
    public List<ScheduleAggregate> Schedules { get; } = [];

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

file class AddCourtFakeUnitOfWork : IUnitOfWork
{
    public bool SaveChangesCalled { get; private set; }

    public Task SaveChangesAsync()
    {
        SaveChangesCalled = true;
        return Task.CompletedTask;
    }
}

public class AddCourtToScheduleHandlerTests
{
    [Fact]
    public void CreateCommand_InvalidScheduleId_ReturnsFailure()
    {
        var result = AddCourtToScheduleCommand.Create("not-a-guid", "S1");

        Assert.IsType<Result<AddCourtToScheduleCommand>.Failure>(result);
    }

    [Fact]
    public void CreateCommand_InvalidCourtId_ReturnsFailure()
    {
        var scheduleId = Guid.NewGuid().ToString();
        var result = AddCourtToScheduleCommand.Create(scheduleId, "X99");

        Assert.IsType<Result<AddCourtToScheduleCommand>.Failure>(result);
    }

    [Fact]
    public async Task HandleAsync_ScheduleNotFound_ReturnsNotFoundFailure()
    {
        var repo = new AddCourtFakeScheduleRepo();
        var uow = new AddCourtFakeUnitOfWork();
        var handler = new AddCourtToScheduleHandler(repo, uow);
        var command = ((Result<AddCourtToScheduleCommand>.Success)
            AddCourtToScheduleCommand.Create(Guid.NewGuid().ToString(), "S1")).Value;

        var result = await handler.HandleAsync(command);

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.ErrorType == ErrorType.NotFound);
        Assert.False(uow.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_AddsCourtAndSaves()
    {
        var repo = new AddCourtFakeScheduleRepo();
        var uow = new AddCourtFakeUnitOfWork();
        var handler = new AddCourtToScheduleHandler(repo, uow);

        var schedule = ScheduleAggregate.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        await repo.AddSchedule(schedule);

        var command = ((Result<AddCourtToScheduleCommand>.Success)
            AddCourtToScheduleCommand.Create(schedule.Id.ToString(), "S1")).Value;

        var result = await handler.HandleAsync(command);

        Assert.IsType<Result<None>.Success>(result);
        Assert.Contains(repo.Schedules[0].Courts, c => c == CourtId.Create("S1").Payload);
        Assert.True(uow.SaveChangesCalled);
    }
}

