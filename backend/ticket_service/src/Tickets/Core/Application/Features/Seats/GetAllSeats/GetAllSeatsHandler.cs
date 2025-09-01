using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.Seats.GetAllSeats;

public class GetAllSeatsHandler : IRequestHandler<GetAllSeatsQuery, Result<List<GetAllSeatsResponse>>>
{
    private readonly ISeatRepository _seatRepository;

    public GetAllSeatsHandler(ISeatRepository seatRepository)
    {
        _seatRepository = seatRepository;
    }

    public async Task<Result<List<GetAllSeatsResponse>>> Handle(GetAllSeatsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var seats = _seatRepository.GetAll();

            var response = seats.Select(seat => new GetAllSeatsResponse
            {
                IdSeat = seat.IdSeat,
                Row = seat.Row,
                Number = seat.Number,
                Type = seat.Type,
                Direction = seat.Direction,
                Status = seat.Status,
                IdZone = seat.IdZone,
                ZoneName = seat.IdZoneNavigation?.Name
            }).ToList();

            return Result<List<GetAllSeatsResponse>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<List<GetAllSeatsResponse>>.Failure($"An error occurred while retrieving seats: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}
