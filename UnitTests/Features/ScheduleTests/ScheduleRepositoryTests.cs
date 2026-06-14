using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Domain.Aggregates.Schedules;
using VIAPadelClub.Infrastructure.EfcDomainPersistence;
using VIAPadelClub.Infrastructure.EfcDomainPersistence.Repositories;
namespace UnitTests.Features.ScheduleTests;


public class ScheduleRepositoryTests
{
    private DomainModelContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<DomainModelContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new DomainModelContext(options);
    }

    [Fact]
    public async Task GetAsync_ReturnsSchedule_WhenItExistsInDatabase()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var repository = new ScheduleRepositoryEfc(context);
        
        var schedule = Schedule.Create().Payload;
        
        // We use the raw context to seed the database before testing the repository
        context.Add(schedule);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAsync(schedule.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(schedule.Id, result.Id);
    }
    

    [Fact]
    public async Task AddAsync_SavesScheduleToDatabase()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var repository = new ScheduleRepositoryEfc(context);
        var schedule = Schedule.Create().Payload;

        // Act 
        await repository.AddAsync(schedule);
        await context.SaveChangesAsync(); 

        // Assert
        var scheduleInDb = await context.Set<Schedule>().FirstOrDefaultAsync(s => s.Id == schedule.Id);
        
        Assert.NotNull(scheduleInDb);
        Assert.Equal(schedule.Id, scheduleInDb.Id);
    }
}
