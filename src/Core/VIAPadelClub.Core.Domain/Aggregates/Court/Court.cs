using VIAPadelClub.Core.Domain.Aggregates.Court.Entities;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Aggregates.Court;

public sealed class Court
{
    public CourtId Id { get; }

    private readonly List<Booking> _bookings = [];
    public IReadOnlyList<Booking> Bookings => _bookings.AsReadOnly();

    private Court(CourtId id)
    {
        Id = id;
    }
    
    public static Result<Court> Create(string courtId)
    {
        return CourtId.Create(courtId) switch
        {
            Result<CourtId>.Success s => new Court(s.Value),
            Result<CourtId>.Failure f => Result.Failure<Court>(f.Errors),
            _ => throw new InvalidOperationException()
        };
    }

    internal Result<BookingId> AddBooking(TimeInterval timeInterval, Guid scheduleId, ViaEmail email)
    {
        var id = BookingId.New();
        _bookings.Add(new Booking(id, timeInterval, scheduleId, email));
        return id;
    }
    
    public Result<None> CancelBooking(BookingId bookingId, DateTime currentTime)
    {
        var booking = _bookings.FirstOrDefault(b => b.Id == bookingId);

        if (booking is null)
        {
            return Result.Failure<None>(
                new ResultError("Booking not found.", ErrorType.NotFound));
        }

        if (booking.IsCancelled)
        {
            return Result.Failure<None>(
                new ResultError("Booking is already cancelled.", ErrorType.Validation));
        }

        var start = booking.TimeInterval.Start;

        if (start <= currentTime)
        {
            return Result.Failure<None>(
                new ResultError("Past bookings cannot be cancelled.", ErrorType.Validation));
        }

        var sameDay = currentTime.Date == start.Date;
        var lessThanOneHour = (start - currentTime) < TimeSpan.FromHours(1);

        if (sameDay && lessThanOneHour)
        {
            return Result.Failure<None>(
                new ResultError("Booking cannot be cancelled less than one hour before start time.",
                    ErrorType.Validation));
        }

        booking.Cancel();

        return Result.Success();
    }
}