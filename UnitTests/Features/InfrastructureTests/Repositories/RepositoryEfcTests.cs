using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Domain.Aggregates.Courts;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.ValueObjects;
using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Players;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Infrastructure.EfcDomainPersistence;
using VIAPadelClub.Infrastructure.EfcDomainPersistence.Repositories;

namespace UnitTests.Features.InfrastructureTests.Repositories;

file sealed class EmailAvailableChecker : IEmailInUseChecker
{
    public bool IsEmailInUse(ViaEmail email) => false;
}

public class RepositoryEfcTests
{
    private static DomainModelContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DomainModelContext>()
            .UseInMemoryDatabase($"repo-tests-{Guid.NewGuid()}")
            .Options;

        return new DomainModelContext(options);
    }

    private static Player CreateValidPlayer(string email = "123456@via.dk")
    {
        var viaEmail = ((Result<ViaEmail>.Success)ViaEmail.CreateEmail(email)).Value;
        var name = ((Result<Name>.Success)Name.CreateName("Alex", "Andersen")).Value;
        var image = ((Result<ImageUrl>.Success)ImageUrl.CreateImageUrl("https://via.dk/pic.png")).Value;
        return ((Result<Player>.Success)Player.Register(viaEmail, name, image, new EmailAvailableChecker())).Value;
    }

    [Fact]
    public async Task ScheduleRepository_AddAndGet_ReturnsPersistedSchedule()
    {
        await using var context = CreateContext();
        var repo = new ScheduleRepositoryEfc(context);
        var schedule = ((Result<Schedule>.Success)Schedule.Create()).Value;

        await repo.AddAsync(schedule);
        await context.SaveChangesAsync();

        var retrieved = await repo.GetAsync(schedule.Id);

        Assert.NotNull(retrieved);
        Assert.Equal(schedule.Id, retrieved.Id);
    }

    [Fact]
    public async Task CourtRepository_AddAndGet_ReturnsPersistedCourt()
    {
        await using var context = CreateContext();
        var repo = new CourtRepositoryEfc(context);
        var court = ((Result<Court>.Success)Court.Create("S1")).Value;

        await repo.AddAsync(court);
        await context.SaveChangesAsync();

        var retrieved = await repo.GetAsync(court.Id);

        Assert.NotNull(retrieved);
        Assert.Equal(court.Id, retrieved.Id);
    }

    [Fact]
    public async Task PlayerRepository_AddAndGet_ReturnsPersistedPlayer()
    {
        await using var context = CreateContext();
        var repo = new PlayerRepositoryEfc(context);
        var player = CreateValidPlayer();

        await repo.AddAsync(player);
        await context.SaveChangesAsync();

        var retrieved = await repo.GetAsync(player.Email);

        Assert.NotNull(retrieved);
        Assert.Equal(player.Email, retrieved.Email);
        Assert.Equal("Alex", retrieved.Name.FirstName);
        Assert.Equal("Andersen", retrieved.Name.LastName);
    }
}
