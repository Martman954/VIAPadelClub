using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Application.AppEntry.CourtCommands;

public sealed class CancelBookingCommand
{
    public BookingId BookingId { get; }
    public ViaEmail Email { get; }

    private CancelBookingCommand(BookingId bookingId, ViaEmail email)
    {
        BookingId = bookingId;
        Email = email;
    }

    public static Result<CancelBookingCommand> Create(string bookingId, string email)
    {
        var bookingIdResult = Guid.TryParse(bookingId, out var bookingGuid)
            ? Result.Success(BookingId.From(bookingGuid))
            : Result.Failure<BookingId>(new ResultError("Invalid booking id format.", ErrorType.Validation));

        var emailResult = ViaEmail.CreateEmail(email);

        return Result
            .CombineResultsInto<CancelBookingCommand>(bookingIdResult, emailResult)
            .WithPayloadIfSuccess(() => new CancelBookingCommand(
                bookingIdResult.Payload,
                emailResult.Payload));
    }
}

