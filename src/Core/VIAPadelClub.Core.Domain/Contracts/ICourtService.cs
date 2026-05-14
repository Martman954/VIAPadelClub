namespace VIAPadelClub.Core.Domain.Contracts;

public interface ICourtService
{
    Task<Aggregates.Court.Court> GetCourtById(Guid courtId);
}