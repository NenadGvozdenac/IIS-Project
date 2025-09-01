using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;
using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Application.Features.Seats.CreateSeat;

public class CreateSeatHandler : IRequestHandler<CreateSeatCommand, Result<CreateSeatResponse>>
{
    private readonly ISeatRepository _seatRepository;
    private readonly IZoneRepository _zoneRepository;

    public CreateSeatHandler(ISeatRepository seatRepository, IZoneRepository zoneRepository)
    {
        _seatRepository = seatRepository;
        _zoneRepository = zoneRepository;
    }

    public async Task<Result<CreateSeatResponse>> Handle(CreateSeatCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate zone exists if provided
            if (request.IdZone.HasValue)
            {
                var zone = _zoneRepository.GetById(request.IdZone.Value);
                if (zone == null)
                {
                    return Result<CreateSeatResponse>.Failure($"Zone with ID {request.IdZone.Value} not found")
                        .WithCode((int)ResultCode.BadRequest);
                }
            }

            var seat = new Seat
            {
                Row = request.Row,
                Number = request.Number,
                Type = request.Type,
                Direction = request.Direction,
                Status = request.Status,
                IdZone = request.IdZone
            };

            var createdSeat = _seatRepository.Create(seat);

            var response = new CreateSeatResponse
            {
                IdSeat = createdSeat.IdSeat,
                Row = createdSeat.Row,
                Number = createdSeat.Number,
                Type = createdSeat.Type,
                Direction = createdSeat.Direction,
                Status = createdSeat.Status,
                IdZone = createdSeat.IdZone
            };

            return Result<CreateSeatResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<CreateSeatResponse>.Failure($"An error occurred while creating the seat: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}
