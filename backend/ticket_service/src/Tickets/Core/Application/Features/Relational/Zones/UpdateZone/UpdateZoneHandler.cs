using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Zones.UpdateZone;

public class UpdateZoneHandler : IRequestHandler<UpdateZoneCommand, Result<UpdateZoneResponse>>
{
    private readonly IZoneRepository _zoneRepository;

    public UpdateZoneHandler(IZoneRepository zoneRepository)
    {
        _zoneRepository = zoneRepository;
    }

    public Task<Result<UpdateZoneResponse>> Handle(UpdateZoneCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingZone = _zoneRepository.GetById(request.IdZone);
            if (existingZone == null)
            {
                return Task.FromResult(Result<UpdateZoneResponse>.Failure($"Zone with ID {request.IdZone} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Task.FromResult(Result<UpdateZoneResponse>.Failure("Zone name is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (!request.Rank.HasValue)
            {
                return Task.FromResult(Result<UpdateZoneResponse>.Failure("Zone rank is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (!request.MaximumCapacity.HasValue || request.MaximumCapacity.Value <= 0)
            {
                return Task.FromResult(Result<UpdateZoneResponse>.Failure("Maximum capacity is required and must be greater than 0")
                    .WithCode((int)ResultCode.BadRequest));
            }

            existingZone.Name = request.Name ?? existingZone.Name;
            existingZone.Rank = request.Rank.Value;
            existingZone.MaximumCapacity = request.MaximumCapacity.Value;
            existingZone.Status = request.Status ?? existingZone.Status;

            var updatedZone = _zoneRepository.Update(existingZone);

            var response = new UpdateZoneResponse
            {
                IdZone = updatedZone.IdZone,
                Name = updatedZone.Name,
                Rank = updatedZone.Rank,
                MaximumCapacity = updatedZone.MaximumCapacity,
                Status = updatedZone.Status
            };

            return Task.FromResult(Result<UpdateZoneResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<UpdateZoneResponse>.Failure($"An error occurred while updating the zone: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
