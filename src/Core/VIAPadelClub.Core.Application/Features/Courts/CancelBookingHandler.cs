using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.CourtCommands;
using VIAPadelClub.Core.Domain.Contracts.Courts;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Application.Features.Courts;

internal class CancelBookingHandler(IBookingCourtFinder bookingCourtFinder) : ICommandHandler<CancelBookingCommand>
{
    public async Task<Result> HandleAsync(CancelBookingCommand command)
    {
        var bookingId = command.BookingId;
        var courtWithBooking = await bookingCourtFinder.FindCourtWithBooking(bookingId);

        if (courtWithBooking is null)
            return Result.Failure("Booking not found.", ErrorType.NotFound);

        var booking = courtWithBooking.Bookings.FirstOrDefault(b => b.Id == bookingId);
        if (booking is null)
            return Result.Failure("Booking not found.", ErrorType.NotFound);

        if (booking.PlayerEmail != command.Email)
            return Result.Failure("Booking does not belong to the specified player.", ErrorType.Validation);

        var cancelResult = courtWithBooking.CancelBooking(bookingId, DateTime.Now);
        if (cancelResult is Result<None>.Failure f)
            return Result.Failure<None>(f.Errors);


        return Result.Success();
    }
}



