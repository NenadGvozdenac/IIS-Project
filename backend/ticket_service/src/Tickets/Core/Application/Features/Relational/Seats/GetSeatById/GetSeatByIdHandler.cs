using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Seats.GetSeatById;

public class GetSeatByIdHandler : IRequestHandler<GetSeatByIdQuery, Result<GetSeatByIdResponse>>
{
    private readonly ISeatRepository _seatRepository;

    public GetSeatByIdHandler(ISeatRepository seatRepository)
    {
        _seatRepository = seatRepository;
    }

    public Task<Result<GetSeatByIdResponse>> Handle(GetSeatByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var seat = _seatRepository.GetById(request.IdSeat);
            if (seat == null)
            {
                return Task.FromResult(Result<GetSeatByIdResponse>.Failure($"Seat with ID {request.IdSeat} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var response = new GetSeatByIdResponse
            {
                IdSeat = seat.IdSeat,
                Row = seat.Row,
                Number = seat.Number,
                Type = seat.Type,
                Direction = seat.Direction,
                Status = seat.Status,
                IdZone = seat.IdZone,
                ZoneName = seat.IdZoneNavigation?.Name
            };

            return Task.FromResult(Result<GetSeatByIdResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetSeatByIdResponse>.Failure($"An error occurred while retrieving the seat: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
