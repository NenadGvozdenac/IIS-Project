using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Seats.GetSeatsByZone;

public class GetSeatsByZoneHandler : IRequestHandler<GetSeatsByZoneQuery, Result<List<GetSeatsByZoneResponse>>>
{
    private readonly ISeatRepository _seatRepository;

    public GetSeatsByZoneHandler(ISeatRepository seatRepository)
    {
        _seatRepository = seatRepository;
    }

    public Task<Result<List<GetSeatsByZoneResponse>>> Handle(GetSeatsByZoneQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var seats = _seatRepository.GetByZoneId(request.ZoneId);

            var response = seats.Select(seat => new GetSeatsByZoneResponse
            {
                IdSeat = seat.IdSeat,
                SeatRow = seat.Row,
                SeatNumber = seat.Number,
                SeatType = seat.Type,
                SeatDirection = seat.Direction,
                SeatStatus = seat.Status,
                IdZone = seat.IdZone ?? 0,
                ZoneName = seat.IdZoneNavigation?.Name
            }).ToList();

            return Task.FromResult(Result<List<GetSeatsByZoneResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<List<GetSeatsByZoneResponse>>.Failure($"An error occurred while retrieving seats for zone {request.ZoneId}: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
