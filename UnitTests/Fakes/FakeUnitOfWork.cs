using VIAPadelClub.Core.Domain.UnitOfWork;

namespace UnitTests.Fakes;

internal class FakeUnitOfWork : IUnitOfWork
{
    public bool SaveChangesCalled { get; private set; }

    public Task SaveChangesAsync()
    {
        SaveChangesCalled = true;
        return Task.CompletedTask;
    }
}