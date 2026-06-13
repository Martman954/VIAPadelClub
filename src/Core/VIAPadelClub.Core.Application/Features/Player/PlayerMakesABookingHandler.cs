using Features.CommandDispatch;
using Features.CommandDispatch.PlayerCommands;
using VIAPadelClub.Core.Domain.Aggregates.Court;
using VIAPadelClub.Core.Domain.Aggregates.Court.Entities;
using VIAPadelClub.Core.Domain.Contracts.Court;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.Features.Player;

internal class PlayerMakesABookingHandler: ICommandHandler<PlayerMakesABookingCommand>
{
    private readonly ICourtRepo _courtRepo;
    private readonly IUnitOfWork _unitOfWork;
    private ICourtHasBookingChecker _courtHasBookingChecker;
    

    internal PlayerMakesABookingHandler(ICourtRepo courtRepo, IUnitOfWork unitOfWork, ICourtHasBookingChecker courtHasBookingChecker)
    {
        _courtRepo = courtRepo;
        _unitOfWork = unitOfWork;
        _courtHasBookingChecker = courtHasBookingChecker;
    }

    public async Task<Result> HandleAsync(PlayerMakesABookingCommand command)
    {
        Result<Booking> newBooking = Court.Crea
        
        
        
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}