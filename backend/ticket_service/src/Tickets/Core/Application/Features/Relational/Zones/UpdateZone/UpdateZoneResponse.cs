namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Zones.UpdateZone;

public class UpdateZoneResponse
{
    public int IdZone { get; set; }
    public string? Name { get; set; }
    public int? Rank { get; set; }
    public int? MaximumCapacity { get; set; }
    public string? Status { get; set; }
}
