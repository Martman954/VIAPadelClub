namespace VIAPadelClub.Core.Domain.UnitOfWork;

public interface IUnitOfWork
{
    public Task SaveChangesAsync();
}