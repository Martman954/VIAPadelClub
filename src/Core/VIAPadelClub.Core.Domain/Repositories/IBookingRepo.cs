using VIAPadelClub.Core.Domain.Aggregates.Court.Entities;

namespace VIAPadelClub.Core.Domain.Repositories;

public interface IBookingRepo
{
    public Task<Booking> AddBooking(Booking player);
    public Task<Booking> GetBooking(Guid playerId);
    public Task<Booking> RemoveBooking(Guid playerId);
   
}