using Microsoft.EntityFrameworkCore;

namespace VIAPadelClub.Infrastructure.SqliteDomainPersistence;

public class SqliteDomainContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SqliteDomainContext).Assembly);
    }
}