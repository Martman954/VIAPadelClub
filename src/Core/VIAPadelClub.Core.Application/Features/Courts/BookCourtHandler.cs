using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.AppEntry.CourtCommands;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Courts;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.Services;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;
using CourtAggregate = VIAPadelClub.Core.Domain.Aggregates.Courts.Court;

namespace VIAPadelClub.Core.Application.Features.Courts;

internal class BookCourtHandler(
    IPlayerRepository playerRepo,
    IScheduleRepository scheduleRepo,
    ICourtRepository courtRepo,
    ICourtHasBookingChecker bookingChecker)
    : ICommandHandler<BookCourtCommand>
{
    public async Task<Result> HandleAsync(BookCourtCommand command)
    {
        // Retrieve Player
        var playerResult = await Result.Try(() => playerRepo.GetAsync(command.PlayerId));
        if (playerResult is Result<VIAPadelClub.Core.Domain.Aggregates.Players.Player>.Failure)
            return Result.Failure("Player not found.", ErrorType.NotFound);

        // Retrieve Schedule
        var scheduleResult = await Result.Try(() => scheduleRepo.GetAsync(ScheduleId.From(command.ScheduleId)));
        if (scheduleResult is Result<VIAPadelClub.Core.Domain.Aggregates.Schedules.Schedule>.Failure)
            return Result.Failure("Schedule not found.", ErrorType.NotFound);

        // Retrieve Court aggregate with all its bookings loaded from the database
        var courtResult = await Result.Try(() => courtRepo.GetAsync(command.CourtId));
        if (courtResult is Result<CourtAggregate>.Failure)
            return Result.Failure("Court not found.", ErrorType.NotFound);

        var player = playerResult.Payload;
        var schedule = scheduleResult.Payload;
        var court = courtResult.Payload;

        if (player == null || schedule == null || court == null)
            return Result.Failure("Player, Schedule, or Court not found.", ErrorType.NotFound);

        // Create BookingRequest and call domain service
        var bookingRequest = new BookingRequest(player, court, schedule, command.TimeInterval);
        var bookingResult = BookCourtInSchedule.Handle(bookingRequest, DateTime.Now, bookingChecker);

        if (bookingResult is Result<BookingId>.Failure bf)
            return Result.Failure<None>(bf.Errors);


        return Result.Success();
    }
}





