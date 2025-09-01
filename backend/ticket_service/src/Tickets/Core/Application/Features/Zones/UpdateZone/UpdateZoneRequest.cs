namespace ticket_service.src.Tickets.Core.Application.Features.Zones.UpdateZone;

public class UpdateZoneRequest
{
    public string? Name { get; set; }
    public int? Rank { get; set; }
    public int? MaximumCapacity { get; set; }
    public string? Status { get; set; }
}
