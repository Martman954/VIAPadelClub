using VIAPadelClub.Core.Domain.Aggregates.Court;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Domain.Contracts;

public interface ICourtService
{
    Task<Result<Court>> GetCourt(CourtId courtId);
    Task<Result<Court>> GetCourtByBooking(BookingId bookingId);
    Task<Result<List<Court>>> GetCourtsBySchedule(Guid scheduleId);
    
    
}