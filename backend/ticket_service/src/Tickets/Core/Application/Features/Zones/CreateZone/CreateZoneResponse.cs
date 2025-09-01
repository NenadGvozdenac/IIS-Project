namespace ticket_service.src.Tickets.Core.Application.Features.Zones.CreateZone;

public class CreateZoneResponse
{
    public int IdZone { get; set; }
    public string? Name { get; set; }
    public int? Rank { get; set; }
    public int? MaximumCapacity { get; set; }
    public string? Status { get; set; }
}
