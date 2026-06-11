using VIAPadelClub.Core.Application.CommandDispatch;
using VIAPadelClub.Core.Application.CommandDispatch.CourtCommands;
using VIAPadelClub.Core.Domain.Contracts.Courts;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Application.Features.Courts;

internal class CancelBookingHandler : ICommandHandler<CancelBookingCommand>
{
    private readonly IBookingCourtFinder _bookingCourtFinder;
    private readonly IUnitOfWork _unitOfWork;

    internal CancelBookingHandler(
        IBookingCourtFinder bookingCourtFinder,
        IUnitOfWork unitOfWork)
    {
        _bookingCourtFinder = bookingCourtFinder;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(CancelBookingCommand command)
    {
        var bookingId = command.BookingId;
        var courtWithBooking = await _bookingCourtFinder.FindCourtWithBooking(bookingId);

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

        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}



