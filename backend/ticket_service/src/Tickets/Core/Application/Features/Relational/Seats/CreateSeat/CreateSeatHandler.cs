using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;
using ticket_service.src.Tickets.Core.Domain.Entities.Relational;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Seats.CreateSeat;

public class CreateSeatHandler : IRequestHandler<CreateSeatCommand, Result<CreateSeatResponse>>
{
    private readonly ISeatRepository _seatRepository;
    private readonly IZoneRepository _zoneRepository;

    public CreateSeatHandler(ISeatRepository seatRepository, IZoneRepository zoneRepository)
    {
        _seatRepository = seatRepository;
        _zoneRepository = zoneRepository;
    }

    public Task<Result<CreateSeatResponse>> Handle(CreateSeatCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate zone exists if provided
            if (request.IdZone.HasValue)
            {
                var zone = _zoneRepository.GetById(request.IdZone.Value);
                if (zone == null)
                {
                    return Task.FromResult(Result<CreateSeatResponse>.Failure($"Zone with ID {request.IdZone.Value} not found")
                        .WithCode((int)ResultCode.BadRequest));
                }
            }

            // Additional validation for required fields
            if (!request.Row.HasValue)
            {
                return Task.FromResult(Result<CreateSeatResponse>.Failure("Seat row is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (!request.Number.HasValue)
            {
                return Task.FromResult(Result<CreateSeatResponse>.Failure("Seat number is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (string.IsNullOrWhiteSpace(request.Type))
            {
                return Task.FromResult(Result<CreateSeatResponse>.Failure("Seat type is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (string.IsNullOrWhiteSpace(request.Direction))
            {
                return Task.FromResult(Result<CreateSeatResponse>.Failure("Seat direction is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (string.IsNullOrWhiteSpace(request.Status))
            {
                return Task.FromResult(Result<CreateSeatResponse>.Failure("Seat status is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            var seat = new Seat
            {
                Row = request.Row.Value,
                Number = request.Number.Value,
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

            return Task.FromResult(Result<CreateSeatResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CreateSeatResponse>.Failure($"An error occurred while creating the seat: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
