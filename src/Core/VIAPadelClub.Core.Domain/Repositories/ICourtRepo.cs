using VIAPadelClub.Core.Domain.Aggregates.Court;

namespace VIAPadelClub.Core.Domain.Repositories;

public interface ICourtRepo
{
    public Task<Court> AddCourt(Court court);
    public Task<Court> GetCourt(Guid courtId);
    public Task<Court> RemoveCourt(Guid courtId);
}