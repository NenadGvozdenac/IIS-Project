using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.Zones.UpdateZone;

public class UpdateZoneHandler : IRequestHandler<UpdateZoneCommand, Result<UpdateZoneResponse>>
{
    private readonly IZoneRepository _zoneRepository;

    public UpdateZoneHandler(IZoneRepository zoneRepository)
    {
        _zoneRepository = zoneRepository;
    }

    public async Task<Result<UpdateZoneResponse>> Handle(UpdateZoneCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingZone = _zoneRepository.GetById(request.IdZone);
            if (existingZone == null)
            {
                return Result<UpdateZoneResponse>.Failure($"Zone with ID {request.IdZone} not found")
                    .WithCode((int)ResultCode.NotFound);
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Result<UpdateZoneResponse>.Failure("Zone name is required")
                    .WithCode((int)ResultCode.BadRequest);
            }

            if (request.MaximumCapacity.HasValue && request.MaximumCapacity.Value <= 0)
            {
                return Result<UpdateZoneResponse>.Failure("Maximum capacity must be greater than 0")
                    .WithCode((int)ResultCode.BadRequest);
            }

            existingZone.Name = request.Name;
            existingZone.Rank = request.Rank;
            existingZone.MaximumCapacity = request.MaximumCapacity;
            existingZone.Status = request.Status;

            var updatedZone = _zoneRepository.Update(existingZone);

            var response = new UpdateZoneResponse
            {
                IdZone = updatedZone.IdZone,
                Name = updatedZone.Name,
                Rank = updatedZone.Rank,
                MaximumCapacity = updatedZone.MaximumCapacity,
                Status = updatedZone.Status
            };

            return Result<UpdateZoneResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<UpdateZoneResponse>.Failure($"An error occurred while updating the zone: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}
