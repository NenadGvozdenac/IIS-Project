using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.Seats.UpdateSeat;

public class UpdateSeatHandler : IRequestHandler<UpdateSeatCommand, Result<UpdateSeatResponse>>
{
    private readonly ISeatRepository _seatRepository;
    private readonly IZoneRepository _zoneRepository;

    public UpdateSeatHandler(ISeatRepository seatRepository, IZoneRepository zoneRepository)
    {
        _seatRepository = seatRepository;
        _zoneRepository = zoneRepository;
    }

    public Task<Result<UpdateSeatResponse>> Handle(UpdateSeatCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingSeat = _seatRepository.GetById(request.IdSeat);
            if (existingSeat == null)
            {
                return Task.FromResult(Result<UpdateSeatResponse>.Failure($"Seat with ID {request.IdSeat} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Validate zone exists if provided
            if (request.IdZone.HasValue)
            {
                var zone = _zoneRepository.GetById(request.IdZone.Value);
                if (zone == null)
                {
                    return Task.FromResult(Result<UpdateSeatResponse>.Failure($"Zone with ID {request.IdZone.Value} not found")
                        .WithCode((int)ResultCode.BadRequest));
                }
            }

            existingSeat.Row = request.Row;
            existingSeat.Number = request.Number;
            existingSeat.Type = request.Type;
            existingSeat.Direction = request.Direction;
            existingSeat.Status = request.Status;
            existingSeat.IdZone = request.IdZone;

            var updatedSeat = _seatRepository.Update(existingSeat);

            var response = new UpdateSeatResponse
            {
                IdSeat = updatedSeat.IdSeat,
                Row = updatedSeat.Row,
                Number = updatedSeat.Number,
                Type = updatedSeat.Type,
                Direction = updatedSeat.Direction,
                Status = updatedSeat.Status,
                IdZone = updatedSeat.IdZone
            };

            return Task.FromResult(Result<UpdateSeatResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<UpdateSeatResponse>.Failure($"An error occurred while updating the seat: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
