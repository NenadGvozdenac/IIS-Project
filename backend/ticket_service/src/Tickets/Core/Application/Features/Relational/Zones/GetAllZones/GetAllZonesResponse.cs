namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Zones.GetAllZones;

public class GetAllZonesResponse
{
    public List<ZoneDto> Zones { get; set; } = new List<ZoneDto>();
}

public class ZoneDto
{
    public int IdZone { get; set; }
    public string? Name { get; set; }
    public int? Rank { get; set; }
    public int? MaximumCapacity { get; set; }
    public string? Status { get; set; }
}
