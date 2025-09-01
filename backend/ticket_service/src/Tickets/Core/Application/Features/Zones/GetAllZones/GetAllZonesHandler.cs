using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.Zones.GetAllZones;

public class GetAllZonesHandler : IRequestHandler<GetAllZonesQuery, Result<GetAllZonesResponse>>
{
    private readonly IZoneRepository _zoneRepository;

    public GetAllZonesHandler(IZoneRepository zoneRepository)
    {
        _zoneRepository = zoneRepository;
    }

    public Task<Result<GetAllZonesResponse>> Handle(GetAllZonesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var zones = _zoneRepository.GetAll();

            var response = new GetAllZonesResponse
            {
                Zones = zones.Select(z => new ZoneDto
                {
                    IdZone = z.IdZone,
                    Name = z.Name,
                    Rank = z.Rank,
                    MaximumCapacity = z.MaximumCapacity,
                    Status = z.Status
                }).ToList()
            };

            return Task.FromResult(Result<GetAllZonesResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetAllZonesResponse>.Failure($"An error occurred while retrieving zones: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
