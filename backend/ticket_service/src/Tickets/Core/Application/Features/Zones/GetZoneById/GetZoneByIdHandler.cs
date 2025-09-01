using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.Zones.GetZoneById;

public class GetZoneByIdHandler : IRequestHandler<GetZoneByIdQuery, Result<GetZoneByIdResponse>>
{
    private readonly IZoneRepository _zoneRepository;

    public GetZoneByIdHandler(IZoneRepository zoneRepository)
    {
        _zoneRepository = zoneRepository;
    }

    public async Task<Result<GetZoneByIdResponse>> Handle(GetZoneByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var zone = _zoneRepository.GetById(request.Id);

            if (zone == null)
            {
                return Result<GetZoneByIdResponse>.Failure($"Zone with ID {request.Id} not found")
                    .WithCode((int)ResultCode.NotFound);
            }

            var response = new GetZoneByIdResponse
            {
                IdZone = zone.IdZone,
                Name = zone.Name,
                Rank = zone.Rank,
                MaximumCapacity = zone.MaximumCapacity,
                Status = zone.Status
            };

            return Result<GetZoneByIdResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetZoneByIdResponse>.Failure($"An error occurred while retrieving the zone: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}
