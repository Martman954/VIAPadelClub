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
    public async Task ScheduleRepository_AddAsync_WithoutEntityModel_ThrowsInvalidOperationException()
    {
        await using var context = CreateContext();
        var repo = new ScheduleRepositoryEfc(context);
        var schedule = ((Result<Schedule>.Success)Schedule.Create()).Value;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => repo.AddAsync(schedule));
        Assert.Contains("not included in the model", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CourtRepository_AddAsync_WithoutEntityModel_ThrowsInvalidOperationException()
    {
        await using var context = CreateContext();
        var repo = new CourtRepositoryEfc(context);
        var court = ((Result<Court>.Success)Court.Create("S1")).Value;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => repo.AddAsync(court));
        Assert.Contains("not included in the model", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PlayerRepository_AddAsync_WithoutEntityModel_ThrowsInvalidOperationException()
    {
        await using var context = CreateContext();
        var repo = new PlayerRepositoryEfc(context);
        var player = CreateValidPlayer();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => repo.AddAsync(player));
        Assert.Contains("not included in the model", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}


