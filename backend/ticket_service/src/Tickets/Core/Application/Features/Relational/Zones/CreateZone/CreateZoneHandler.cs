using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;
using ticket_service.src.Tickets.Core.Domain.Entities.Relational;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Zones.CreateZone;

public class CreateZoneHandler : IRequestHandler<CreateZoneCommand, Result<CreateZoneResponse>>
{
    private readonly IZoneRepository _zoneRepository;

    public CreateZoneHandler(IZoneRepository zoneRepository)
    {
        _zoneRepository = zoneRepository;
    }

    public Task<Result<CreateZoneResponse>> Handle(CreateZoneCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Task.FromResult(Result<CreateZoneResponse>.Failure("Zone name is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (!request.Rank.HasValue)
            {
                return Task.FromResult(Result<CreateZoneResponse>.Failure("Zone rank is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (!request.MaximumCapacity.HasValue || request.MaximumCapacity.Value <= 0)
            {
                return Task.FromResult(Result<CreateZoneResponse>.Failure("Maximum capacity is required and must be greater than 0")
                    .WithCode((int)ResultCode.BadRequest));
            }

            var zone = new Zone
            {
                Name = request.Name,
                Rank = request.Rank.Value,
                MaximumCapacity = request.MaximumCapacity.Value,
                Status = request.Status ?? "Active"
            };

            var createdZone = _zoneRepository.Create(zone);

            var response = new CreateZoneResponse
            {
                IdZone = createdZone.IdZone,
                Name = createdZone.Name,
                Rank = createdZone.Rank,
                MaximumCapacity = createdZone.MaximumCapacity,
                Status = createdZone.Status
            };

            return Task.FromResult(Result<CreateZoneResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CreateZoneResponse>.Failure($"An error occurred while creating the zone: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
