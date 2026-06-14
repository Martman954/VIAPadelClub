using VIAPadelClub.Core.Application.AppEntry.ScheduleCommands;
using VIAPadelClub.Core.Application.Features.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;
using ScheduleAggregate = VIAPadelClub.Core.Domain.Aggregates.Schedules.Schedule;

namespace UnitTests.Features.ApplicationTests.Schedule;

file class AddCourtFakeScheduleRepo : IScheduleRepository
{
    public List<ScheduleAggregate> Schedules { get; } = [];

    public async Task AddAsync(ScheduleAggregate schedule)
    {
        Schedules.Add(schedule);
        await Task.CompletedTask;
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
        var handler = new AddCourtToScheduleHandler(repo);
        var command = ((Result<AddCourtToScheduleCommand>.Success)
            AddCourtToScheduleCommand.Create(Guid.NewGuid().ToString(), "S1")).Value;

        var result = await handler.HandleAsync(command);

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.ErrorType == ErrorType.NotFound);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_AddsCourt()
    {
        var repo = new AddCourtFakeScheduleRepo();
        var handler = new AddCourtToScheduleHandler(repo);

        var schedule = ScheduleAggregate.Create().Payload;
        schedule.UpdateDate(DateTime.Today.AddDays(1));
        await repo.AddAsync(schedule);

        var command = ((Result<AddCourtToScheduleCommand>.Success)
            AddCourtToScheduleCommand.Create(schedule.Id.GuidValue.ToString(), "S1")).Value;

        var result = await handler.HandleAsync(command);

        Assert.IsType<Result<None>.Success>(result);
        Assert.Contains(repo.Schedules[0].Courts, c => c.Equals(CourtId.Create("S1").Payload));
    }
}

